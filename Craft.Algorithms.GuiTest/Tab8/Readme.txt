Her har vi et HEXAGONALT grid.

Jeg prøvede i første omgang at lave det på samme måde som i tab5, der hvor man manipulerer pixels, specifikt ved at bygge det som
et Canvas med en indlejret ItemsControl konfigureret som UniformGrid

Det virkede dog ikke, da der for et hex grid er behov for at tegne "uden for" cellerne i et uniformt grid

Derfor gjorde jeg i stedet det, at jeg lavede 2 canvases med større celler, så der ikke var behov for at tegne uden for.
Det første (underliggende) canvas indeholder hexagonerne fra de "lige" rækker (0, 2, 4, 6 osv) mens det andet (øverste)
grid indeholder rækkerne fra de "ulige rækker" (1, 3, 5, 7)

Det virkede fint fsa angår at tegne griddet, men jeg havde et gevaldigt bøvl med at sikre, at mouse clicks på polygoner
i det underliggende lag blev registreret, da det øverste lag skærmede for det nederste. Jeg spurgte ChatGPT, som ledte
mig lidt på vildspor, dels ved (tilsyneladende fejlagtigt) at påstå, at det at sætte Background for en control til 
Transparent influerede på om controlen registrerede mouse click events. Den foreslog også, at jeg kunne gøre brug af
IsHitTestVisible, VisualTreeHelper eller en teknik kaldet "tunnelering" af mouse events, men jeg havde ikke held med 
nogen af delene og var i øvrigt ikke vild med at skulle arbejde i code behind filen

Jeg fandt en ret simpel løsning, der blot bestod i at UNDLADE at sætte Background til noget for det grid, jeg brugte
i ItemTemplaten for mine ItemsControls. Det, at jeg overhovedet havde et grid son root element for templaten, skyldtes,
at jeg ville sikre at indexes var korrekt ved at overlejre med noget tekst, og så skulle templaten bestå af et grid 
med en Polygon og en TextBlock, men hvis man bare bruger en Polygon, kan man slippe for at skulle bruge et grid - det
er den der regel der minder om reglen for en UserControl i almindelighed om at man kun kan have et enkelt root element.


