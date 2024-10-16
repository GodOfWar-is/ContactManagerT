using System;

namespace ContactManagerT
{
    internal class Contact
    {
        // 联系人属性
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        // 构造函数
        public Contact(string name, string address, string phone)
        {
            Name = name;
            Address = address;
            Phone = phone;
        }

        // 重写 ToString 方法，以便在 ListBox 或其他地方显示联系人的名称
        public override string ToString()
        {
            return Name; // 返回姓名，方便列表显示
        }
    }
}
