using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Script.Serialization;

namespace Vse.Web.Serialization.Test
{
    public interface ISampleModel : IEnumerable
    {
        CultureInfo CultureInfo { get; set; }
    }
    public class ComplexModel
    {
        public SampleModel SampleModel  { get;set;}
        public DateTime DateTime { get; set; } 
    }

    public class SampleModel: ISampleModel
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

        public static IEnumerable<SampleModel> CreateListSampleWithCircularReference()
        {
            var list = new List<SampleModel>();
            for(int i=0;i<1;i++)
            {
                var item1 = new SampleModel { Name = "a", Number = 1 };
                var item2 = new SampleModel { Name = "b", Number = 2 };
                item1.Child = item2; // circular reference 
                var item3 = new SampleModel { Name = "c", Number = 3 };
                item2.Child = item3;
                item3.Child = item1;
                list.Add(item1);
            }
            return list.ToArray();
        }

        public static SampleModel CreateSampleWithCultureInfo()
        {
            var item1 = CreateSampleWithCircularReference();
            item1.CultureInfo = CultureInfo.CurrentCulture;
            return item1;
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
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
