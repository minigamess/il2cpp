using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace DecompilerTests
{
    // 测试 Attribute
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class TestAttribute : Attribute
    {
        public string Description { get; set; }
        public int Priority { get; set; }

        public TestAttribute(string description)
        {
            Description = description;
        }
    }

    // 测试接口
    public interface ITestInterface
    {
        void InterfaceMethod();
        int InterfaceProperty { get; set; }
    }

    public interface IGenericInterface<T>
    {
        T GetValue();
        void SetValue(T value);
    }

    // 测试基类
    [TestAttribute("Base class for testing")]
    public class BaseTestClass
    {
        protected int m_baseField;
        public virtual void VirtualMethod() { }
        public virtual int VirtualProperty { get; set; }
    }

    // 测试继承和接口实现
    [TestAttribute("Test class with inheritance and interfaces", Priority = 1)]
    [Preserve]
    public class DecompilerTestCases : BaseTestClass, ITestInterface, IGenericInterface<string>
    {
        // 字段
        private int m_privateField;
        public string m_publicField;
        protected List<int> m_protectedList;

        // 属性
        [TestAttribute("Interface property implementation")]
        public int InterfaceProperty { get; set; }

        public override int VirtualProperty
        {
            get => base.VirtualProperty;
            set => base.VirtualProperty = value;
        }

        // 构造函数
        public DecompilerTestCases()
        {
            m_protectedList = new List<int>();
        }

        public DecompilerTestCases(int value) : this()
        {
            m_privateField = value;
        }

        // 接口方法实现
        public void InterfaceMethod()
        {
            Debug.Log("Interface method called");
        }

        // 泛型接口实现
        public string GetValue()
        {
            return m_publicField;
        }

        public void SetValue(string value)
        {
            m_publicField = value;
        }

        // 重写基类方法
        public override void VirtualMethod()
        {
            base.VirtualMethod();
            Debug.Log("Overridden virtual method");
        }

        // 测试 ref 参数
        [TestAttribute("Method with ref parameter")]
        public void MethodWithRef(ref int value)
        {
            value += 10;
            Debug.Log($"Ref parameter modified: {value}");
        }

        // 测试 out 参数
        [TestAttribute("Method with out parameter")]
        public bool MethodWithOut(out string result, out int count)
        {
            result = "Success";
            count = m_protectedList.Count;
            return true;
        }

        // 测试 ref 和 out 组合
        public void MethodWithRefAndOut(ref int input, out int output)
        {
            output = input * 2;
            input += 5;
        }

        // 测试泛型方法 - 无约束
        public T GenericMethod<T>(T value)
        {
            return value;
        }

        // 测试泛型方法 - where T : class 约束
        public void GenericMethodWithClassConstraint<T>(T obj) where T : class
        {
            if (obj != null)
            {
                Debug.Log($"Object: {obj}");
            }
        }

        // 测试泛型方法 - where T : struct 约束
        public T GenericMethodWithStructConstraint<T>(T value) where T : struct
        {
            return value;
        }

        // 测试泛型方法 - where T : new() 约束
        public T GenericMethodWithNewConstraint<T>() where T : new()
        {
            return new T();
        }

        // 测试泛型方法 - where T : 基类约束
        public void GenericMethodWithBaseConstraint<T>(T item) where T : BaseTestClass
        {
            item.VirtualMethod();
        }

        // 测试泛型方法 - where T : 接口约束
        public void GenericMethodWithInterfaceConstraint<T>(T item) where T : ITestInterface
        {
            item.InterfaceMethod();
        }

        // 测试泛型方法 - 多个约束组合
        public void GenericMethodWithMultipleConstraints<T>(T item)
            where T : class, ITestInterface, new()
        {
            var instance = new T();
            instance.InterfaceMethod();
        }

        // 测试泛型类
        [TestAttribute("Generic test class")]
        public class GenericTestClass<T> where T : class, new()
        {
            private T m_value;

            public GenericTestClass()
            {
                m_value = new T();
            }

            public T GetValue()
            {
                return m_value;
            }

            public void SetValue(T value)
            {
                m_value = value;
            }

            // 嵌套泛型方法
            public U Convert<U>(T input) where U : class
            {
                return input as U;
            }
        }

        // 测试嵌套类
        public class NestedClass
        {
            public void NestedMethod()
            {
                Debug.Log("Nested class method");
            }
        }

        // 测试静态方法
        public static void StaticMethod()
        {
            Debug.Log("Static method called");
        }

        // 测试方法重载
        public void OverloadedMethod(int value)
        {
            Debug.Log($"Int: {value}");
        }

        public void OverloadedMethod(string value)
        {
            Debug.Log($"String: {value}");
        }

        public void OverloadedMethod(int value, string text)
        {
            Debug.Log($"Int: {value}, String: {text}");
        }

        // 测试索引器
        public int this[int index]
        {
            get => m_protectedList[index];
            set => m_protectedList[index] = value;
        }

        // 测试事件
        public event Action<int> OnValueChanged;

        protected virtual void RaiseValueChanged(int value)
        {
            OnValueChanged?.Invoke(value);
        }

        // 测试 ref return（类中可以返回字段引用）
        private int m_refReturnField;

        public ref int GetRefReturnField()
        {
            return ref m_refReturnField;
        }
    }

    // 测试结构体实现接口
    [Preserve]
    public struct TestStruct : ITestInterface
    {
        public int Value;

        public void InterfaceMethod()
        {
            Debug.Log($"Struct interface method: {Value}");
        }

        public int InterfaceProperty { get; set; }
    }

    // 测试抽象类
    [TestAttribute("Abstract base class")]
    public abstract class AbstractTestClass
    {
        public abstract void AbstractMethod();
        public virtual void VirtualMethod() { }
    }

    // 测试密封类
    [TestAttribute("Sealed class")]
    public sealed class SealedTestClass : AbstractTestClass
    {
        public override void AbstractMethod()
        {
            Debug.Log("Sealed class implementation");
        }
    }

    // 测试枚举
    [Flags]
    public enum TestEnum
    {
        None = 0,
        Flag1 = 1,
        Flag2 = 2,
        Flag3 = 4,
        All = Flag1 | Flag2 | Flag3
    }

    // 测试委托
    public delegate void TestDelegate(int value);
    public delegate TResult GenericDelegate<T, TResult>(T input);

    // 测试扩展方法
    public static class TestExtensions
    {
        public static void ExtensionMethod(this DecompilerTestCases obj, string message)
        {
            Debug.Log($"Extension method: {message}");
        }

        public static T GenericExtension<T>(this T obj) where T : class
        {
            return obj;
        }
    }
}

