using FlexibleAOPComponent;
using System.Collections.Generic;

namespace TestClient
{
    public class ButtonMgr : ObjectWithAspects
    {
        public static int k = 0;
        public int AddButton(int i)
        {
            k++;
            return i++;
        }

        public int DivButton(int i)
        {
            return i / 0;
        }
        public List<int> MultiParametersMethod(int p1, string p2, List<int> p3)
        {
            return new List<int>() { p1, p1, p1 };
        }
    }
}
