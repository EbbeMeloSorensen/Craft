using System.Collections;
using Craft.DataStructures.Geometry;
using Craft.Logging;
using Craft.Math;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries;
using Craft.Simulation.Boundaries.Interfaces;

namespace Craft.Simulation.Engine
{
    // Denne klasse er stateless. Den indeholder den matematik, der bruges til at fremskrive tilstanden for et fysisk system og
    // bruges af EngineCore-klassen, som i øvrigt er den, der holder staten
    public static class Calculator
    {
        private enum StateEvent
        {
            None,
            CollisionWithBoundary,
            CollisionBetweenBodies
        }

        public static State PropagateState(
            Scene scene,
            State state,
            double deltaT,
            ILogger logger,
            out List<BoundaryCollisionReport> boundaryCollisionReports,
            out List<BodyCollisionReport> bodyCollisionReports) // Dictionary of body id vs effective normal vectors of boundary collisions that took place in the propagation
        {
            if (scene == null)
            {
                throw new InvalidOperationException("Please set a scene before calling EngineCore.PropagateState");
            }

            boundaryCollisionReports = new List<BoundaryCollisionReport>();
            bodyCollisionReports = new List<BodyCollisionReport>();

            var timeLeftInCurrentIncrement = deltaT;
            var idsOfHandledBodies = new HashSet<int>();
            var handledCollisions = new HashSet<Tuple<int, int>>();
            var indexOfInputState = state.Index;

            if (logger.IsEnabled)
            {
                logger.WriteLine(
                    LogMessageCategory.Debug,
                    $"Propagating state {indexOfInputState}:", "propagation");
            }

            var iteration = 1;
            while (timeLeftInCurrentIncrement > 1E-12)
            {
                if (iteration > 1000)
                {
                    // trouble
                    var a = 0;
                }

                // Beregn positionsforskydninger givet de gældende kræfter (hvor vi vel at mærke ikke tager højde for boundaries)
                var propagatedBodyStateMap = CalculatePropagatedBodyStateMap(
                    state,
                    scene.StandardGravity,
                    scene.GravitationalConstant,
                    scene.CoefficientOfFriction,
                    scene.IncludeCustomForces,
                    timeLeftInCurrentIncrement,
                    idsOfHandledBodies);

                BodyState bodyState = null;
                IBoundary boundary = null;
                var timeUntilCollisionWithBoundary = double.NaN;
                Vector2D lineSegmentEndPointInvolvedInCollision = null;
                Vector2D effectiveSurfaceNormalForBoundary = null;

                if (scene.HandleBoundaryCollisions)
                {
                    IdentifyFirstCollisionWithABoundary(
                        propagatedBodyStateMap,
                        scene.Boundaries,
                        scene.BoundaryDataSource,
                        timeLeftInCurrentIncrement,
                        out bodyState,
                        out boundary,
                        out timeUntilCollisionWithBoundary,
                        out lineSegmentEndPointInvolvedInCollision,
                        out effectiveSurfaceNormalForBoundary);
                }

                BodyState bodyState1 = null;
                BodyState bodyState2 = null;
                var timeUntilCollisionBetweenBodies = double.NaN;
                Vector2D doorPointInvolvedInDoorCollision = null;
                Vector2D effectiveSurfaceNormalForDoorCollision = null;

                if (scene.HandleBodyCollisions)
                {
                    IdentifyFirstCollisionBetweenTwoBodies(
                        propagatedBodyStateMap,
                        scene.CheckForCollisionBetweenBodiesCallback,
                        idsOfHandledBodies,
                        handledCollisions,
                        timeLeftInCurrentIncrement,
                        out bodyState1,
                        out bodyState2,
                        out timeUntilCollisionBetweenBodies,
                        out doorPointInvolvedInDoorCollision,
                        out effectiveSurfaceNormalForDoorCollision);
                }

                StateEvent stateEvent;

                if (double.IsNaN(timeUntilCollisionWithBoundary))
                {
                    stateEvent = double.IsNaN(timeUntilCollisionBetweenBodies)
                        ? StateEvent.None
                        : StateEvent.CollisionBetweenBodies;
                }
                else
                {
                    if (double.IsNaN(timeUntilCollisionBetweenBodies))
                    {
                        stateEvent = StateEvent.CollisionWithBoundary;
                    }
                    else
                    {
                        stateEvent = timeUntilCollisionWithBoundary < timeUntilCollisionBetweenBodies
                            ? StateEvent.CollisionWithBoundary
                            : StateEvent.CollisionBetweenBodies;
                    }
                }

                var timeElapsed = double.NaN;

                switch (stateEvent)
                {
                    case StateEvent.None:
                        state = new State(propagatedBodyStateMap.Keys.ToList());
                        timeLeftInCurrentIncrement = 0.0;

                        if (logger.IsEnabled)
                        {
                            logger.WriteLine(
                                LogMessageCategory.Debug,
                                $"  Iteration {iteration}, progress: 100%", "propagation");

                            logger.WriteLine(
                                LogMessageCategory.Debug,
                                "    Result:", "propagation");

                            LogState(state, logger);
                        }

                        break;
                    case StateEvent.CollisionWithBoundary:
                        PropagateStatePartly(propagatedBodyStateMap, timeUntilCollisionWithBoundary, timeLeftInCurrentIncrement);
                        boundaryCollisionReports.Add(new BoundaryCollisionReport(bodyState, boundary, effectiveSurfaceNormalForBoundary));

                        // Figure out what should happen with the body that collided with a boundary by asking the scene
                        if (scene.CollisionBetweenBodyAndBoundaryOccuredCallBack != null)
                        {
                            var outcome = scene.CollisionBetweenBodyAndBoundaryOccuredCallBack.Invoke(bodyState.Body);

                            switch (outcome)
                            {
                                case OutcomeOfCollisionBetweenBodyAndBoundary.Block:
                                    {
                                        propagatedBodyStateMap[bodyState].EliminateVelocityComponentTowardsGivenSurfaceNormal(effectiveSurfaceNormalForBoundary);
                                        state = new State(propagatedBodyStateMap.Values.ToList());
                                        idsOfHandledBodies.Add(bodyState.Body.Id);
                                        break;
                                    }
                                case OutcomeOfCollisionBetweenBodyAndBoundary.Reflect:
                                    {
                                        propagatedBodyStateMap[bodyState].ReflectVelocity(boundary, lineSegmentEndPointInvolvedInCollision, effectiveSurfaceNormalForBoundary);
                                        state = new State(propagatedBodyStateMap.Values.ToList());
                                        //idsOfHandledBodies.Add(bodyState.Body.Id);
                                        break;
                                    }
                                default:
                                    throw new ArgumentException();
                            }
                        }
                        else
                        {
                            // The scene doesn't have an opinion, but it actually should
                            throw new InvalidOperationException("No callback for handling collision between body and boundary was provided by the scene");
                        }

                        timeElapsed = timeUntilCollisionWithBoundary;
                        timeLeftInCurrentIncrement -= timeElapsed;

                        if (logger.IsEnabled)
                        {
                            logger.WriteLine(
                                LogMessageCategory.Debug,
                                $"  Body{bodyState.Body.Id} collided with boundary after {timeUntilCollisionWithBoundary} seconds. Time Left: {timeLeftInCurrentIncrement} seconds", "propagation");

                            logger.WriteLine(
                                LogMessageCategory.Debug,
                                $"  Iteration {iteration} progress: {100 * (deltaT - timeLeftInCurrentIncrement) / deltaT:F5}%", "propagation");

                            logger.WriteLine(
                                LogMessageCategory.Debug,
                                "    Result:", "propagation");

                            LogState(state, logger);
                        }

                        break;
                    case StateEvent.CollisionBetweenBodies:
                        PropagateStatePartly(propagatedBodyStateMap, timeUntilCollisionBetweenBodies, timeLeftInCurrentIncrement);
                        bodyCollisionReports.Add(new BodyCollisionReport(bodyState1.Body, bodyState2.Body));

                        // Figure out what should happen with the two bodies that collided by asking the scene
                        if (scene.CollisionBetweenTwoBodiesOccuredCallBack != null)
                        {
                            var outcome = scene.CollisionBetweenTwoBodiesOccuredCallBack.Invoke(
                                bodyState1.Body, bodyState2.Body);

                            switch (outcome)
                            {
                                case OutcomeOfCollisionBetweenTwoBodies.Block:
                                    {
                                        // Dette var det gamle mode, som kun sjældent er brugt i praksis - og sikkert bare i lab regi
                                        //propagatedBodyStateMap[bodyState1].NaturalVelocity = new Vector2D(0, 0);
                                        //propagatedBodyStateMap[bodyState2].NaturalVelocity = new Vector2D(0, 0);
                                        // Dette er det nye mode, der indtil videre kun bruges til kollision med en dør 
                                        propagatedBodyStateMap[bodyState1].EliminateVelocityComponentTowardsGivenSurfaceNormal(effectiveSurfaceNormalForDoorCollision);
                                        state = new State(propagatedBodyStateMap.Values.ToList());
                                        idsOfHandledBodies.Add(bodyState1.Body.Id);
                                        idsOfHandledBodies.Add(bodyState2.Body.Id);
                                        break;
                                    }
                                case OutcomeOfCollisionBetweenTwoBodies.ElasticCollision:
                                    {
                                        propagatedBodyStateMap[bodyState1].HandleElasticCollision(propagatedBodyStateMap[bodyState2]);
                                        state = new State(propagatedBodyStateMap.Values.ToList());
                                        idsOfHandledBodies.Add(bodyState1.Body.Id);
                                        idsOfHandledBodies.Add(bodyState2.Body.Id);
                                        break;
                                    }
                                case OutcomeOfCollisionBetweenTwoBodies.Ignore:
                                    {
                                        state = new State(propagatedBodyStateMap.Values.ToList());
                                        handledCollisions.Add(new Tuple<int, int>(
                                            System.Math.Min(bodyState1.Body.Id, bodyState2.Body.Id),
                                            System.Math.Max(bodyState1.Body.Id, bodyState2.Body.Id)));

                                        //idsOfHandledBodies.Add(bodyState1.Body.Id);
                                        //idsOfHandledBodies.Add(bodyState2.Body.Id);
                                        break;
                                    }
                                default:
                                    throw new ArgumentException();
                            }
                        }
                        else
                        {
                            // The scene doesn't have an opinion, but it actually should
                            throw new InvalidOperationException("No callback for handling collision between two bodies was provided by the scene");
                        }

                        timeElapsed = timeUntilCollisionBetweenBodies;
                        timeLeftInCurrentIncrement -= timeElapsed;

                        if (logger.IsEnabled)
                        {
                            logger.WriteLine(
                                LogMessageCategory.Debug,
                                $"  Body{bodyState1.Body.Id} and Body{bodyState2.Body.Id} collided after {timeUntilCollisionBetweenBodies} seconds. Time Left: {timeLeftInCurrentIncrement} seconds", "propagation");

                            logger.WriteLine(
                                LogMessageCategory.Debug,
                                $"  Iteration {iteration} progress: {100 * (deltaT - timeLeftInCurrentIncrement) / deltaT:F5}%", "propagation");
                        }

                        break;
                    default:
                        throw new ArgumentException("Invalid state event");
                }

                if (timeElapsed > 1e-8)
                {
                    idsOfHandledBodies.Clear();
                }

                iteration++;
            }

            state.Index = indexOfInputState + 1;

            return state;
        }

        public static void LogState(
            State state,
            ILogger logger)
        {
            if (!logger.IsEnabled) return;

            logger.WriteLine(
                LogMessageCategory.Debug,
                $"      Energy: {state.CalculateTotalEnergy(10.0)}", "propagation");

            logger.WriteLine(
                LogMessageCategory.Debug,
                "      Bodies:", "propagation");

            state.BodyStates.ForEach(bs =>
            {
                logger.WriteLine(
                    LogMessageCategory.Debug,
                    $"        Body{bs.Body.Id}: Position: ({bs.Position.X}, {bs.Position.Y}, Natural Velocity: ({bs.NaturalVelocity.X}, {bs.NaturalVelocity.Y}))", "propagation");
            });
        }

        // Tager en tilstand som input og beregner den tilstand, der gør sig gældende på et senere tidspunkt, givet de positioner, hastigheder og kræfter, der gælder.
        // Bemærk, at metoden ikke tager højde for boundaries af nogen art
        // Bemærk også, at bodies, der er markeret som værende "håndteret", hvilket indebærer, at de allerede har fået tildelt en position og hastighed, bibeholder deres position og hastighed
        // Key er den propagerede, og value er den oprindelige
        private static Dictionary<BodyState, BodyState> CalculatePropagatedBodyStateMap(
            State state,
            double standardGravity,
            double gravitationalConstant,
            double coefficientOfFriction,
            bool includeCustomForces,
            double timeLeftInCurrentIncrement,
            HashSet<int> idsOfHandledBodies)
        {
            var forceMap =
                state.BodyStates.ToDictionary(bs => bs, bs => new Vector2D(0, 0));

            if (System.Math.Abs(coefficientOfFriction) > 0.0001)
            {
                state.BodyStates.ForEach(bs =>
                {
                    if (System.Math.Abs(bs.NaturalVelocity.Length) < 0.00001) return;

                    var force = bs.Body.Mass * coefficientOfFriction;

                    var acceleration = force / bs.Body.Mass;
                    var speed = bs.NaturalVelocity.Length;
                    var nextSpeed = speed - timeLeftInCurrentIncrement * acceleration;

                    // Ensure that the frictional force wont cause the body to reverse its direction of movement
                    if (nextSpeed < 0.0) return;

                    var forceDirection = -bs.NaturalVelocity.Normalize();

                    forceMap[bs] += force * forceDirection;
                });
            }

            if (System.Math.Abs(standardGravity) > 0.0001)
            {
                var forceDirection = new Vector2D(0, 1);

                state.BodyStates.ForEach(bs =>
                {
                    if (bs.Body.AffectedByGravity)
                    {
                        forceMap[bs] += bs.Body.Mass * standardGravity * forceDirection;
                    }
                });
            }

            if (System.Math.Abs(gravitationalConstant) > 1E-15)
            {
                var innerLoopSkipCount = 1;

                state.BodyStates.ForEach(bs1 =>
                {
                    state.BodyStates.Skip(innerLoopSkipCount).ToList().ForEach(bs2 =>
                    {
                        var body1 = bs1.Body;
                        var body2 = bs2.Body;
                        var bodyState1 = bs1;
                        var bodyState2 = bs2;

                        var vectorFrom1To2 = bodyState2.Position - bodyState1.Position;
                        var distance = vectorFrom1To2.Length;

                        if (System.Math.Abs(distance) < 0.0001)
                        {
                            // Gravitational force would become ridiculously large
                            return;
                        }

                        vectorFrom1To2 = vectorFrom1To2.Normalize();

                        var force = gravitationalConstant * body1.Mass * body2.Mass * vectorFrom1To2 / System.Math.Pow(distance, 2);

                        forceMap[bodyState1] += force;
                        forceMap[bodyState2] -= force;
                    });

                    innerLoopSkipCount++;
                });
            }

            if (includeCustomForces)
            {
                state.BodyStates.ForEach(bs =>
                {
                    var bsc = bs as BodyStateClassic;

                    if (bsc == null)
                    {
                        return;
                    }

                    forceMap[bs] += bs.Body.Mass * bsc.EffectiveCustomForce;
                });
            }

            return state.BodyStates.ToDictionary(
                _ => idsOfHandledBodies.Contains(_.Body.Id)
                    ? _.Clone()
                    : _.Propagate(timeLeftInCurrentIncrement, forceMap[_]),
                _ => _.Clone());
        }

        private static void IdentifyFirstCollisionBetweenTwoBodies(
            Dictionary<BodyState, BodyState> propagatedBodyStateMap,
            CheckForCollisionBetweenBodiesCallback checkForCollisionBetweenBodiesCallback,
            HashSet<int> idsOfHandledBodies,
            HashSet<Tuple<int, int>> handledBodyCollisions,
            double timeLeftInCurrentIncrement,
            out BodyState? bodyState1InvolvedInCollision,
            out BodyState? bodyState2InvolvedInCollision,
            out double timeUntilCollision,
            out Vector2D doorPointInvolvedInCollision,
            out Vector2D effectiveSurfaceNormalForDoor)
        {
            bodyState1InvolvedInCollision = null;
            bodyState2InvolvedInCollision = null;
            timeUntilCollision = double.NaN;
            doorPointInvolvedInCollision = null;
            effectiveSurfaceNormalForDoor = null;

            // Denne bruger vi til at sikre, at vi ikke håndterer b vs a efter at have håndteret a vs b
            // som hvis det var en trace matrix, hvor vi så kun håndterede den ene trekant
            var innerLoopSkipCount = 1;

            foreach (var kvp1 in propagatedBodyStateMap)
            {
                var bs1Before = kvp1.Value;
                var bs1After = kvp1.Key;
                var body1 = bs1Before.Body;

                if (idsOfHandledBodies.Contains(body1.Id))
                {
                    continue;
                }

                foreach (var kvp2 in propagatedBodyStateMap.Skip(innerLoopSkipCount++))
                {
                    var bs2Before = kvp2.Value;
                    var bs2After = kvp2.Key;
                    var body2 = bs2Before.Body;

                    if (body1 is Projectile || body2 is Projectile)
                    {
                        var a = 0;
                    }

                    if (body1 is BodyDoor &&
                        body2 is BodyDoor)
                    {
                        // Dette burde ikke ske, men det gør det - måske fordi Skip ikke opfører sig som du tror..
                        var a = 0;
                    }

                    if (idsOfHandledBodies.Contains(body2.Id))
                    {
                        continue;
                    }

                    if (checkForCollisionBetweenBodiesCallback != null)
                    {
                        // Det er afhænger af scenens opsætning, om vi overhovedet skal checke for
                        // kollision mellem disse 2 bodies. Sædvanligvis afhænger det af, hvilke
                        // typer af bodies der er tale om.
                        if (!checkForCollisionBetweenBodiesCallback.Invoke(body1, body2))
                        {
                            continue;
                        }
                    }

                    if (handledBodyCollisions.Any())
                    {
                        if (handledBodyCollisions.Contains(new Tuple<int, int>(
                            System.Math.Min(body1.Id, body2.Id),
                            System.Math.Max(body1.Id, body2.Id))))
                        {
                            continue;
                        }
                    }

                    if (body1 is CircularBody &&
                        body2 is CircularBody)
                    {
                        var radius1 = (body1 as CircularBody).Radius;
                        var radius2 = (body2 as CircularBody).Radius;
                        var radiusSum = radius1 + radius2;

                        var vectorFrom1To2Before = bs2Before.Position - bs1Before.Position;
                        var distanceBefore = vectorFrom1To2Before.Length;

                        if (radiusSum >= distanceBefore)
                        {
                            // Trouble intersection at the start of the iteration
                            // Det sker for Pool table, many balls, selv om den ellers virker stabil..
                            var a = 0;
                        }

                        var vectorFrom1To2After = bs2After.Position - bs1After.Position;
                        var distanceAfter = vectorFrom1To2After.Length;

                        if (radiusSum < distanceAfter)
                        {
                            // no collision
                            continue;
                        }

                        // There is a collision between the two circles
                        var p1 = bs1Before.Position;
                        var p2 = bs2Before.Position;
                        var v1 = bs1Before.NaturalVelocity;
                        var v2 = bs2Before.NaturalVelocity;

                        var t = Operations.TimeOfCollisionBetweenTwoCircles(
                            p1.X,
                            p1.Y,
                            p2.X,
                            p2.Y,
                            v1.X,
                            v1.Y,
                            v2.X,
                            v2.Y,
                            radius1 + 0.000000000001,
                            radius2);

                        if (double.IsNaN(timeUntilCollision) ||
                            t < timeUntilCollision)
                        {
                            bodyState1InvolvedInCollision = bs1After;
                            bodyState2InvolvedInCollision = bs2After;
                            timeUntilCollision = t;
                        }
                    }
                    else
                    {
                        if ((body1 is CircularBody && body2 is BodyDoor) ||
                            (body2 is CircularBody && body1 is BodyDoor))
                        {
                            var bsCircularBodyAfter = body1 is CircularBody
                                ? bs1After
                                : bs2After;

                            var bsDoor = body1 is BodyDoor
                                ? bs1After as BodyStateDoor
                                : bs2After as BodyStateDoor;

                            // Lav en LineSegment boundary som repræsentant for døren, for den kan vi allerede håndtere

                            var circularBody = bsCircularBodyAfter.Body as CircularBody;
                            var door = bsDoor.Body as BodyDoor;

                            Vector2D point2 = door.Point2;

                            if (bsDoor.PercentageOpen > 99)
                            {
                                var angle = (bsDoor.PercentageOpen) * 0.5 * System.Math.PI / 100;

                                var doorAsVector = new Vector2D(
                                    door.Point2.X - door.Point1.X,
                                    door.Point2.Y - door.Point1.Y);

                                var doorWidth = doorAsVector.Length;
                                var hatted = doorAsVector.Hat();

                                if (!bsDoor.OpenClockWise)
                                {
                                    hatted = -hatted;
                                }

                                var pt2_x =
                                    door.Point1.X +
                                    System.Math.Cos(angle) * doorAsVector.X +
                                    System.Math.Sin(angle) * hatted.X;

                                var pt2_y =
                                    door.Point1.Y +
                                    System.Math.Cos(angle) * doorAsVector.Y +
                                    System.Math.Sin(angle) * hatted.Y;

                                point2 = new Vector2D(pt2_x, pt2_y);
                            }

                            var lineSegment = new LineSegment(
                                door.Point1,
                                point2);

                            if (!lineSegment.Intersects(bsCircularBodyAfter))
                            {
                                // No intersection
                                continue;
                            }

                            // The circular body intersects the door 

                            var bsCircularBodyBefore = body1 is CircularBody
                                ? bs1Before
                                : bs2Before;

                            var effectiveVelocity = (bsCircularBodyAfter.Position - bsCircularBodyBefore.Position) / timeLeftInCurrentIncrement;

                            Vector2D effectiveSurfaceNormalForCollisionWithDoor = null;

                            var velocityComponentTowardsBoundary = System.Math.Abs(lineSegment.ProjectVectorOntoSurfaceNormal(effectiveVelocity));

                            var tNew = double.NaN;
                            Vector2D doorEndPointInvolvedInCollisionWithDoor = null;

                            if (velocityComponentTowardsBoundary <= 0)
                            {
                                // Bodyens hastighed er parallel med væggen, hvilket er ensbetydende med at 
                                // den har ramt en af liniestykkets ender
                                doorEndPointInvolvedInCollisionWithDoor =
                                    lineSegment.IsVectorPointingInSameDirectionAsLineSegmentVector(
                                        effectiveVelocity)
                                        ? lineSegment.Point1
                                        : lineSegment.Point2;
                            }
                            else
                            {
                                var vPointOnLineToBodyCenter = bsCircularBodyBefore.Position - lineSegment.Point1;
                                var distanceFromBodyCenterToLineForLineSegment = System.Math.Abs(Vector2D.DotProduct(lineSegment.SurfaceNormal, vPointOnLineToBodyCenter));
                                tNew = (distanceFromBodyCenterToLineForLineSegment - (circularBody.Radius + 0.000000000001)) / velocityComponentTowardsBoundary;

                                if (tNew < 0.0)
                                {
                                    // Trouble
                                    var a = 0;
                                }

                                var backtrackedPosition = bsCircularBodyBefore.Position + effectiveVelocity * tNew;

                                // Nu skal vi så finde ud af, om dette punkt er tættest på liniestykket eller et af dens endepunkter
                                //var lineSegmentPart = lineSegment.ClosestPartOfLineSegment(backtrackedPosition);
                                var lineSegmentPart = lineSegment.ClosestPartOfLineSegment(backtrackedPosition);

                                switch (lineSegmentPart)
                                {
                                    case LineSegmentPart.Point1:
                                        doorEndPointInvolvedInCollisionWithDoor = lineSegment.Point1;
                                        break;
                                    case LineSegmentPart.Point2:
                                        doorEndPointInvolvedInCollisionWithDoor = lineSegment.Point2;
                                        break;
                                    case LineSegmentPart.MiddleSection:
                                        effectiveSurfaceNormalForCollisionWithDoor = Vector2D.DotProduct(effectiveVelocity, lineSegment.SurfaceNormal) < 0
                                            ? lineSegment.SurfaceNormal
                                            : -lineSegment.SurfaceNormal;
                                        break;
                                }

                                if (doorEndPointInvolvedInCollisionWithDoor != null)
                                {
                                    var p1 = bsCircularBodyBefore.Position;
                                    var p2 = doorEndPointInvolvedInCollisionWithDoor;
                                    var v1 = effectiveVelocity;
                                    var v2 = new Vector2D(0, 0);
                                    var radius1 = circularBody.Radius;
                                    var radius2 = 0.0;

                                    tNew = Operations.TimeOfCollisionBetweenTwoCircles(
                                        p1.X,
                                        p1.Y,
                                        p2.X,
                                        p2.Y,
                                        v1.X,
                                        v1.Y,
                                        v2.X,
                                        v2.Y,
                                        radius1 + 0.000000000001,
                                        radius2);

                                    if (tNew < 0.0)
                                    {
                                        // Trouble
                                        var a = 0;
                                    }

                                    var circleCenterAtTimeOfCollision =
                                        bsCircularBodyBefore.Position + tNew * effectiveVelocity;

                                    effectiveSurfaceNormalForCollisionWithDoor = (circleCenterAtTimeOfCollision - doorEndPointInvolvedInCollisionWithDoor).Normalize();
                                }

                                if (double.IsNaN(timeUntilCollision) ||
                                    tNew < timeUntilCollision)
                                {
                                    // The collision happens earlier than any other collision identified so far,
                                    // so we update the output parameters
                                    timeUntilCollision = tNew;
                                    bodyState1InvolvedInCollision = bsCircularBodyAfter;
                                    bodyState2InvolvedInCollision = bsDoor;
                                    doorPointInvolvedInCollision = doorEndPointInvolvedInCollisionWithDoor;
                                    effectiveSurfaceNormalForDoor = effectiveSurfaceNormalForCollisionWithDoor;
                                }
                            }
                        }
                        else
                        {
                            throw new NotImplementedException("No support for detecting collisions betweeen these body types");
                        }
                    }
                }
            }
        }

        private static void IdentifyFirstCollisionWithABoundary(
            Dictionary<BodyState, BodyState> propagatedBodyStateMap,
            List<IBoundary> boundaries,
            IGeometryDataSource boundaryDataStore,
            double timeLeftInCurrentIncrement,
            out BodyState bodyStateInvolvedInCollision,
            out IBoundary boundaryInvolvedInCollision,
            out double timeUntilCollision,
            out Vector2D lineSegmentEndPointInvolvedInCollision,
            out Vector2D effectiveSurfaceNormalForBoundary)
        {
            bodyStateInvolvedInCollision = null;
            boundaryInvolvedInCollision = null;
            lineSegmentEndPointInvolvedInCollision = null;
            timeUntilCollision = double.NaN;
            effectiveSurfaceNormalForBoundary = null;
            
            foreach (var kvp in propagatedBodyStateMap)
            {
                var body = kvp.Key.Body;

                if (!(body is CircularBody || body is RectangularBody))
                {
                    // Only circular bodies and rectangular bodies may collide with a boundary
                    continue; 
                }

                if (!body.AffectedByBoundaries)
                {
                    continue;
                }

                var bsBefore = kvp.Value;
                var bsAfter = kvp.Key;

                var effectiveVelocity = (bsAfter.Position - bsBefore.Position) / timeLeftInCurrentIncrement;

                IEnumerable potentiallyIntersectingBoundaries = boundaries;

                if (boundaryDataStore != null)
                {
                    var bodySize = bsAfter.Body switch
                    {
                        CircularBody circularBody => new Size2D(circularBody.Radius, circularBody.Radius),
                        RectangularBody rectangularBody => new Size2D(rectangularBody.Width / 2, rectangularBody.Height / 2),
                        _ => throw new ArgumentException()
                    };

                    var boundingBoxOfBody = new BoundingBox(
                        bsAfter.Position.X - bodySize.Width,
                        bsAfter.Position.X + bodySize.Width,
                        bsAfter.Position.Y - bodySize.Height,
                        bsAfter.Position.Y + bodySize.Height);

                    potentiallyIntersectingBoundaries = boundaryDataStore.Query(boundingBoxOfBody);
                }

                foreach (var temp in potentiallyIntersectingBoundaries)
                {
                    var boundary = temp as IBoundary;

                    if (boundary.Intersects(bsBefore))
                    {
                        // Trouble..
                        var a = 0;
                    }

                    if (!boundary.Intersects(bsAfter))
                    {
                        continue;
                    }

                    Vector2D effectiveSurfaceNormalForCurrentBoundary = null;

                    if (boundary is ILineSegment)
                    {
                        var lineSegment = boundary as ILineSegment;

                        var velocityComponentTowardsBoundary = System.Math.Abs(lineSegment.ProjectVectorOntoSurfaceNormal(effectiveVelocity));

                        var tNew = double.NaN;
                        Vector2D lineSegmentEndPointInvolvedInCollisionForCurrentBoundary = null;

                        if (velocityComponentTowardsBoundary <= 0)
                        {
                            // Bodyens hastighed er parallel med væggen, hvilket er ensbetydende med at 
                            // den har ramt en af liniestykkets ender
                            lineSegmentEndPointInvolvedInCollisionForCurrentBoundary =
                                lineSegment.IsVectorPointingInSameDirectionAsLineSegmentVector(
                                    effectiveVelocity)
                                    ? lineSegment.Point1
                                    : lineSegment.Point2;
                        }
                        else
                        {
                            // Hvis vi er her, har vi at gøre med det generelle tilfælde, hvor kuglens hastighed IKKE parallel med liniestykket,
                            // men vi ved endnu ikke, om vi har ramt liniestykkets SIDE eller en af dens 2 ENDER.
                            // Det finder vi ud af ved at føre kuglen tilbage langs dens hastighedsvektor indtil den tangerer
                            // den linie, som definerer liniestykket.

                            // Backtrack the body to where the line that defines the line segment
                            // is a tangent to the circle
                            // Todo: Undersøg om det kan gøres polymorfisk frem for at bruge en switch case ladder
                            switch (body)
                            {
                                case CircularBody circularBody:
                                    {
                                        var vPointOnLineToBodyCenter = bsBefore.Position - lineSegment.Point1;
                                        var distanceFromBodyCenterToLineForLineSegment = System.Math.Abs(Vector2D.DotProduct(lineSegment.SurfaceNormal, vPointOnLineToBodyCenter));
                                        tNew = (distanceFromBodyCenterToLineForLineSegment - (circularBody.Radius + 0.000000000001)) / velocityComponentTowardsBoundary;

                                        if (tNew < 0.0)
                                        {
                                            // Trouble
                                            // Det sker for ball train I, selv om den ellers virker stabil..
                                            var a = 0;
                                        }

                                        // Nu regner vi så lige ud, hvor kuglens centrum ville være, hvis vi førte den tilbage med dette t
                                        //var backtrackedPosition = bsAfter.Position - effectiveVelocity * t;
                                        var backtrackedPosition2 = bsBefore.Position + effectiveVelocity * tNew;

                                        // Nu skal vi så finde ud af, om dette punkt er tættest på liniestykket eller et af dens endepunkter
                                        //var lineSegmentPart = lineSegment.ClosestPartOfLineSegment(backtrackedPosition);
                                        var lineSegmentPart = lineSegment.ClosestPartOfLineSegment(backtrackedPosition2);

                                        switch (lineSegmentPart)
                                        {
                                            case LineSegmentPart.Point1:
                                                lineSegmentEndPointInvolvedInCollisionForCurrentBoundary = lineSegment.Point1; ;
                                                break;
                                            case LineSegmentPart.Point2:
                                                lineSegmentEndPointInvolvedInCollisionForCurrentBoundary = lineSegment.Point2; ;
                                                break;
                                            case LineSegmentPart.MiddleSection:
                                                effectiveSurfaceNormalForCurrentBoundary = Vector2D.DotProduct(effectiveVelocity, lineSegment.SurfaceNormal) < 0
                                                    ? lineSegment.SurfaceNormal
                                                    : -lineSegment.SurfaceNormal;
                                                break;
                                        }
                                        break;
                                    }
                                case RectangularBody rectangularBody:
                                    {
                                        // Lige som for en cirkulær body skal vi her føre rektanglet tilbage indtil det tangerer den linie, der definerer liniestykket.
                                        // Hvis der efterfølgende gælder, at den stadig rører liniestykket, så har den ramt SIDEN af liniestykket, og ellers har den
                                        // ramt en af liniestykkets endepunkter.

                                        // Hvad med at lave en generel metode for BodyState, der beregner dens position efter tilbageføring langs hastighedsvektoren
                                        // Så kan du også gøre det polymorfisk i stedet for at bruge en switch case ladder
                                        var overshootDistance = lineSegment.CalculateOvershootDistance(bsAfter);
                                        var buffer = 0.000001; // Backtrack an additional micro meter to ensure we don't have intersection due to rounding errors

                                        var t = (overshootDistance + buffer) / velocityComponentTowardsBoundary;

                                        var backtrackedPosition = bsAfter.Position - effectiveVelocity * t;

                                        switch (lineSegment)
                                        {
                                            case HorizontalLineSegment horizontalLineSegment:
                                                {
                                                    var x0 = backtrackedPosition.X - rectangularBody.Width / 2;
                                                    var x1 = backtrackedPosition.X + rectangularBody.Width / 2;

                                                    if (x1 < horizontalLineSegment.X0)
                                                    {
                                                        lineSegmentEndPointInvolvedInCollisionForCurrentBoundary = lineSegment.Point1;
                                                        effectiveSurfaceNormalForCurrentBoundary = new Vector2D(-1, 0);
                                                    }
                                                    else if (x0 > horizontalLineSegment.X1)
                                                    {
                                                        lineSegmentEndPointInvolvedInCollisionForCurrentBoundary = lineSegment.Point2;
                                                        effectiveSurfaceNormalForCurrentBoundary = new Vector2D(1, 0);
                                                    }
                                                    else
                                                    {
                                                        effectiveSurfaceNormalForCurrentBoundary = bsBefore.Velocity.Y > 0
                                                            ? new Vector2D(0, -1)
                                                            : new Vector2D(0, 1);
                                                    }

                                                    break;
                                                }
                                            case VerticalLineSegment verticalLineSegment:
                                                {
                                                    var y0 = backtrackedPosition.Y - rectangularBody.Height / 2;
                                                    var y1 = backtrackedPosition.Y + rectangularBody.Height / 2;

                                                    if (y1 < verticalLineSegment.Y0)
                                                    {
                                                        lineSegmentEndPointInvolvedInCollisionForCurrentBoundary = lineSegment.Point1;
                                                        effectiveSurfaceNormalForCurrentBoundary = new Vector2D(0, -1);
                                                    }
                                                    else if (y0 > verticalLineSegment.Y1)
                                                    {
                                                        lineSegmentEndPointInvolvedInCollisionForCurrentBoundary = lineSegment.Point2;
                                                        effectiveSurfaceNormalForCurrentBoundary = new Vector2D(0, 1);
                                                    }
                                                    else
                                                    {
                                                        effectiveSurfaceNormalForCurrentBoundary = bsBefore.Velocity.X > 0
                                                            ? new Vector2D(-1, 0)
                                                            : new Vector2D(1, 0);
                                                    }

                                                    break;
                                                }
                                            default:
                                                {
                                                    throw new InvalidOperationException("Unable to handle collision");
                                                }
                                        }

                                        break;
                                    }
                            }
                        }

                        if (lineSegmentEndPointInvolvedInCollisionForCurrentBoundary != null)
                        {
                            // Hvis vi er her, har kuglen ramt en af liniestykkets ender, evt med en hastighedsvektor parallel med liniestykket.
                            // Et evt beregnet t i blokken ovenfor kan ikke bruges, så vi skal beregne t her

                            // Todo: Undersøg om det kan gøres polymorfisk frem for at bruge en switch case ladder
                            switch (bsAfter.Body)
                            {
                                case CircularBody circularBody:
                                    {
                                        var p1 = bsBefore.Position;
                                        var p2 = lineSegmentEndPointInvolvedInCollisionForCurrentBoundary;
                                        var v1 = effectiveVelocity;
                                        var v2 = new Vector2D(0, 0);
                                        var radius1 = circularBody.Radius;
                                        var radius2 = 0.0;

                                        tNew = Operations.TimeOfCollisionBetweenTwoCircles(
                                            p1.X,
                                            p1.Y,
                                            p2.X,
                                            p2.Y,
                                            v1.X,
                                            v1.Y,
                                            v2.X,
                                            v2.Y,
                                            radius1 + 0.000000000001,
                                            radius2);

                                        if (tNew < 0.0)
                                        {
                                            // Trouble
                                            var a = 0;
                                        }

                                        var circleCenterAtTimeOfCollision =
                                            bsBefore.Position + tNew * effectiveVelocity;

                                        effectiveSurfaceNormalForCurrentBoundary = (circleCenterAtTimeOfCollision - lineSegmentEndPointInvolvedInCollisionForCurrentBoundary).Normalize();

                                        break;
                                    }
                                case RectangularBody rectangularBody:
                                    {
                                        // Du skal vist ca gøre det samme som når du regner på kollision mellem rectangular body og punkt

                                        var x = lineSegmentEndPointInvolvedInCollisionForCurrentBoundary.X;
                                        var y = lineSegmentEndPointInvolvedInCollisionForCurrentBoundary.Y;
                                        var x0 = bsAfter.Position.X - rectangularBody.Width / 2;
                                        var x1 = bsAfter.Position.X + rectangularBody.Width / 2;
                                        var y0 = bsAfter.Position.Y - rectangularBody.Height / 2;
                                        var y1 = bsAfter.Position.Y + rectangularBody.Height / 2;

                                        // Hvor lang tid siden er det at punktet intersektede med en af de lodrette akser?
                                        var vx = bsBefore.Velocity.X;
                                        var vy = bsBefore.Velocity.Y;

                                        var tx = double.MaxValue;
                                        var ty = double.MaxValue;

                                        var buffer = 0.000001; // Backtrack an additional micro meter to ensure we don't have intersection due to rounding errors

                                        // Beregn også den normalvektoren for boundaryen, der gør sig gældende for kollisionen
                                        // sikr, at tx ikke bliver negativ
                                        if (vx > 0)
                                        {
                                            tx = (x1 - x + buffer) / vx;
                                        }
                                        else if (vx < 0)
                                        {
                                            tx = (x0 - x - buffer) / vx;
                                        }

                                        if (vy > 0)
                                        {
                                            ty = (y1 - y + buffer) / vy;
                                        }
                                        else if (vy < 0)
                                        {
                                            ty = (y0 - y - buffer) / vy;
                                        }

                                        effectiveSurfaceNormalForCurrentBoundary = ty < tx
                                            ? vy > 0 ? new Vector2D(0, -1) : new Vector2D(0, 1)
                                            : vx > 0 ? new Vector2D(-1, 0) : new Vector2D(1, 0);

                                        var t = ty < tx ? ty : tx;
                                        break;
                                    }
                            }
                        }

                        if (double.IsNaN(timeUntilCollision) ||
                            tNew < timeUntilCollision)
                        {
                            // The collision happens earlier than any other collision identified so far,
                            // so we update the output parameters
                            timeUntilCollision = tNew;
                            bodyStateInvolvedInCollision = bsAfter;
                            boundaryInvolvedInCollision = boundary;
                            lineSegmentEndPointInvolvedInCollision = lineSegmentEndPointInvolvedInCollisionForCurrentBoundary;
                            effectiveSurfaceNormalForBoundary = effectiveSurfaceNormalForCurrentBoundary;
                        }
                    }
                    else if (boundary is IHalfPlane)
                    {
                        // Deprecated (still used for handling collision with half plane)
                        var timeSinceFirstCollisionWithBoundary = double.NaN;

                        // Denne blok håndterer både circular bodies og rectangular bodies
                        var halfPlane = boundary as IHalfPlane;

                        // Dette er længden af hastighedsvektoren projiceret ind på væggens normalvektor.
                        // Den er i praksis negativ, så vi gør den positiv
                        // BEMÆRK: DET HER VIRKER NOK IKKE LÆNGERE, NÅR NU DU PROPAGERER MED ET GENNEMSNIT AF VELOCITY BEFORE OG VELOCITY AFTER
                        //var velocityComponentTowardsBoundary = -halfPlane.ProjectVectorOntoSurfaceNormal(velocityBefore);
                        var velocityComponentTowardsBoundary = System.Math.Abs(halfPlane.ProjectVectorOntoSurfaceNormal(effectiveVelocity));

                        // Hvis denne evaluerer til true er kuglens hastighed parallel med væggen,
                        // så den glider så at sige langs muren
                        // Gør det her egentlig overhovedet nogen forskel?
                        // well det sikrer i hvert fald imod division med 0
                        if (velocityComponentTowardsBoundary <= 0)
                        {
                            continue;
                        }

                        // Tiden er lig med den "dybde", som kuglen har opnået divideret med størrelsen
                        // af dens hastighed i retning af væggen
                        var buffer = 0.000001; // Backtrack an additional micro meter to ensure we don't have intersection due to rounding errors

                        var t = (buffer - halfPlane.DistanceToBody(bsAfter)) / velocityComponentTowardsBoundary;

                        if (!double.IsNaN(timeSinceFirstCollisionWithBoundary) &&
                            !(t > timeSinceFirstCollisionWithBoundary)) continue;

                        // The collision happens earlier than any other collision identified so far,
                        // so we update the output parameters
                        bodyStateInvolvedInCollision = bsAfter;
                        boundaryInvolvedInCollision = boundary;
                        timeSinceFirstCollisionWithBoundary = t;
                        lineSegmentEndPointInvolvedInCollision = null;
                        effectiveSurfaceNormalForBoundary = halfPlane.SurfaceNormal;

                        timeUntilCollision = double.IsNaN(timeSinceFirstCollisionWithBoundary)
                            ? double.NaN
                            : System.Math.Max(0.0, timeLeftInCurrentIncrement - timeSinceFirstCollisionWithBoundary);
                    }
                    else if (boundary is BoundaryPoint)
                    {
                        var boundaryPoint = boundary as BoundaryPoint;
                        double t;

                        switch (body)
                        {
                            case CircularBody circularBody:
                                {
                                    var p1 = bsBefore.Position;
                                    var p2 = boundaryPoint.Point;
                                    var v1 = effectiveVelocity;
                                    var v2 = new Vector2D(0, 0);
                                    var radius1 = circularBody.Radius;
                                    var radius2 = 0.0;

                                    t = Operations.TimeOfCollisionBetweenTwoCircles(
                                        p1.X,
                                        p1.Y,
                                        p2.X,
                                        p2.Y,
                                        v1.X,
                                        v1.Y,
                                        v2.X,
                                        v2.Y,
                                        radius1 + 0.000000000001,
                                        radius2);

                                    break;
                                }
                            case RectangularBody rectangularBody:
                                {
                                    var x = boundaryPoint.Point.X;
                                    var y = boundaryPoint.Point.Y;
                                    var x0 = bsAfter.Position.X - rectangularBody.Width / 2;
                                    var x1 = bsAfter.Position.X + rectangularBody.Width / 2;
                                    var y0 = bsAfter.Position.Y - rectangularBody.Height / 2;
                                    var y1 = bsAfter.Position.Y + rectangularBody.Height / 2;

                                    // Hvor lang tid siden er det at punktet intersektede med en af de lodrette akser?
                                    var vx = bsBefore.NaturalVelocity.X;
                                    var vy = bsBefore.NaturalVelocity.Y;

                                    var tx = double.MaxValue;
                                    var ty = double.MaxValue;

                                    // Beregn også den normalvektoren for boundaryen, der gør sig gældende for kollisionen

                                    var buffer = 0.000001; // Backtrack an additional micro meter to ensure we don't have intersection due to rounding errors

                                    if (vx > 0)
                                    {
                                        tx = (x1 - x + buffer) / vx;
                                    }
                                    else if (vx < 0)
                                    {
                                        tx = (x0 - x - buffer) / vx;
                                    }

                                    if (vy > 0)
                                    {
                                        ty = (y1 - y + buffer) / vy;
                                    }
                                    else if (vy < 0)
                                    {
                                        ty = (y0 - y - buffer) / vy;
                                    }

                                    effectiveSurfaceNormalForCurrentBoundary = ty < tx
                                        ? vy > 0 ? new Vector2D(0, -1) : new Vector2D(0, 1)
                                        : vx > 0 ? new Vector2D(-1, 0) : new Vector2D(1, 0);

                                    t = ty < tx ? ty : tx;

                                    break;
                                }
                            default:
                                throw new ArgumentException();
                        }

                        if (double.IsNaN(timeUntilCollision) ||
                            t < timeUntilCollision)
                        {
                            // The collision happens earlier than any other collision identified so far,
                            // so we update the output parameters
                            bodyStateInvolvedInCollision = bsAfter;
                            boundaryInvolvedInCollision = boundary;
                            timeUntilCollision = t;
                            lineSegmentEndPointInvolvedInCollision = null;

                            var circleCenterAtTimeOfCollision =
                                bsBefore.Position + timeUntilCollision * effectiveVelocity;

                            effectiveSurfaceNormalForBoundary = (circleCenterAtTimeOfCollision - boundaryPoint.Point).Normalize();
                        }
                    }
                    else if (boundary is CircularBoundary)
                    {
                        var circularBoundary = boundary as CircularBoundary;
                        double t;

                        switch (body)
                        {
                            case CircularBody circularBody:
                                {
                                    var p1 = bsBefore.Position;
                                    var p2 = circularBoundary.Center;
                                    var v1 = effectiveVelocity;
                                    var v2 = new Vector2D(0, 0);
                                    var radius1 = circularBody.Radius;
                                    var radius2 = circularBoundary.Radius;

                                    t = Operations.TimeOfCollisionBetweenTwoCircles(
                                        p1.X,
                                        p1.Y,
                                        p2.X,
                                        p2.Y,
                                        v1.X,
                                        v1.Y,
                                        v2.X,
                                        v2.Y,
                                        radius1 + 0.000000000001,
                                        radius2);

                                    break;
                                }
                            case RectangularBody rectangularBody:
                            {
                                throw new NotImplementedException();
                            }
                            default:
                                throw new ArgumentException();
                        }

                        if (double.IsNaN(timeUntilCollision) ||
                            t < timeUntilCollision)
                        {
                            // The collision happens earlier than any other collision identified so far,
                            // so we update the output parameters
                            bodyStateInvolvedInCollision = bsAfter;
                            boundaryInvolvedInCollision = boundary;
                            timeUntilCollision = t;
                            lineSegmentEndPointInvolvedInCollision = null;

                            var circleCenterAtTimeOfCollision =
                                bsBefore.Position + timeUntilCollision * effectiveVelocity;

                            effectiveSurfaceNormalForBoundary = (circleCenterAtTimeOfCollision - circularBoundary.Center).Normalize();
                        }
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }

        // Husk: KEY har den PROPAGEREDE tilstand for hver body, og VALUE-felterne har den OPRINDELIGE 
        private static void PropagateStatePartly(
            Dictionary<BodyState, BodyState> propagatedBodyStateMap,
            double timeInterval,
            double timeLeftInCurrentIncrement)
        {
            var fraction = timeInterval / timeLeftInCurrentIncrement;

            foreach (var kvp in propagatedBodyStateMap)
            {
                var displacement = fraction * (kvp.Key.Position - kvp.Value.Position);
                kvp.Value.Position += displacement;

                kvp.Value.NaturalVelocity += fraction * (kvp.Key.NaturalVelocity - kvp.Value.NaturalVelocity);
            }
        }
    }
}
