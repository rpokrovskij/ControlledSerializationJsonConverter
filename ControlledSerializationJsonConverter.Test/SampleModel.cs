using System;
using System.Globalization;
using System.Web.Script.Serialization;

namespace Vse.Web.Serialization.Test
{
    public class ComplexModel
    {
        public SampleModel SampleModel  { get;set;}
        public DateTime DateTime { get; set; } 
    }

    public class SampleModel
    {
        public static SampleModel CreateSampleWithCircularReference()
        {
            var item1 = new SampleModel { Name = "a", Number = 1 };
            var item2 = new SampleModel { Name = "b", Number = 2 };
            item1.Child = item2; // circular reference 
            var item3 = new SampleModel { Name = "c", Number = 3 };
            item2.Child = item3;
            item3.Child = item1;
            return item1;
        }
        public static SampleModel CreateSampleWithCultureInfo()
        {
            var item1 = CreateSampleWithCircularReference();
            item1.CultureInfo = CultureInfo.CurrentCulture;
            return item1;
        }
        public int Number { get; set; }
        [ScriptIgnore]
        public string Name { get; set; }
        public SampleModel Child { get; set; }
        public CultureInfo CultureInfo { get; set; }

        private string Name2 { get; set; } = "default";

        public string this[int index]
        {
            get
            {
                return Name2[index].ToString();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
