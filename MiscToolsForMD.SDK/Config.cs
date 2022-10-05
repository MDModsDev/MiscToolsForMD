using System.Linq;
using System.Reflection;

namespace MiscToolsForMD.SDK
{
    internal class Config : ISingleOnly
    {
        public bool debug = false;

        public void UpdateFrom(Config from)
        {
            foreach (PropertyInfo propertyInfo in from.GetType().GetProperties())
            {
                if (GetType().GetProperties().Contains(propertyInfo))
                {
                    propertyInfo.SetValue(this, propertyInfo.GetValue(from));
                }
            }
        }
    }
}
