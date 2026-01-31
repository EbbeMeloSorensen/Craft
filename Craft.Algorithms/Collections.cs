namespace Craft.Algorithms
{
    public static class Collections
    {
        public static void CompareWithOtherStringList(
            this List<string> list1,
            List<string> list2,
            out List<string> stringsOnlyPresentInList1,
            out List<string> stringsOnlyPresentInList2)
        {
            stringsOnlyPresentInList1 = new List<string>();
            stringsOnlyPresentInList2 = new List<string>();

            foreach (var s in list1)
            {
                if (!list2.Contains(s))
                {
                    stringsOnlyPresentInList1.Add(s);
                }
            }

            foreach (var s in list2)
            {
                if (!list1.Contains(s))
                {
                    stringsOnlyPresentInList2.Add(s);
                }
            }
        }
    }
}
