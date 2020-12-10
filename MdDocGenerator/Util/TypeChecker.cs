using System;

namespace MdDocGenerator.Util
{
    public class TypeChecker
    {
        public static bool IsGuid(Type type)
        {
            return typeof(Guid) == type || typeof(Guid?) == type;
        }

        public static bool IsString(Type type)
        {
            return typeof(string) == type;
        }

        public static bool IsInt(Type type)
        {
            return typeof(int) == type || typeof(int?) == type;
        }

        public static bool IsDouble(Type type)
        {
            return typeof(double) == type || typeof(double?) == type;
        }

        public static bool IsFloat(Type type)
        {
            return typeof(float) == type || typeof(float?) == type;
        }

        public static bool IsDecimal(Type type)
        {
            return typeof(decimal) == type || typeof(decimal?) == type;
        }

        public static bool IsDateTime(Type type)
        {
            return typeof(DateTime) == type || typeof(DateTime?) == type;
        }

        public static bool IsArray(Type type)
        {
            return typeof(Array) == type;
        }

        public static bool IsClass(Type type)
        {
            return type.IsClass;
        }
    }


    public class MockFactory
    {
        private static IMock _mock;

        public static IMock CreateMock(Type type)
        {
            if (TypeChecker.IsGuid(type))
            {
                _mock = new GuidMock();
            }

            if (TypeChecker.IsInt(type))
            {
                _mock = new IntMock();
            }
            if (TypeChecker.IsFloat(type))
            {
                _mock = new DecimalMock();
            }
            if (TypeChecker.IsDouble(type))
            {
                _mock = new DecimalMock();
            }
            if (TypeChecker.IsDateTime(type))
            {
                _mock = new DateTimeMock();
            }
            if (TypeChecker.IsDecimal(type))
            {
                _mock = new DecimalMock();
            }
            if (TypeChecker.IsString(type))
            {
                _mock = new StringMock();
            }
            if (TypeChecker.IsClass(type))
            {
                _mock = new ClassMock();
            }
            return _mock;
        }
    }

    public class ClassMock : IMock
    {
        public object GetData()
        {
            return null;
        }
    }

    public interface IMock
    {
        object GetData();
    }

    public class GuidMock : IMock
    {
        public object GetData()
        {
            return Guid.NewGuid().ToString("N");
        }
    }

    public class IntMock : IMock
    {
        public object GetData()
        {
            return new Random().Next(99999).ToString();
        }
    }

    public class DecimalMock : IMock
    {
        public object GetData()
        {
            return new Random().Next(99999).ToString("F");
        }
    }

    public class DateTimeMock : IMock
    {
        public object GetData()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
    }

    public class StringMock : IMock
    {
        public object GetData()
        {
            return "任意字符";
        }
    }

}