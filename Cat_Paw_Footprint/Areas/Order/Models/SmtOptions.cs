namespace Cat_Paw_Footprint.Areas.Order.Models
{
    public class SmtpOptions
    {
        // 兩組帳號
        public SmtpAccount Primary { get; set; } = new();
        public SmtpAccount Secondary { get; set; } = new();

        // 🔹 新增：目前使用中的帳號
        private SmtpAccount _active;
        public SmtpAccount Active
        {
            get => _active ?? Primary; // 預設用 Primary
            set
            {
                if (value != null)
                {
                    // 將目前帳號內容同步回屬性，讓 SendAsync 仍然可用 _opt.Host 取值
                    Host = value.Host;
                    Port = value.Port;
                    EnableSsl = value.EnableSsl;
                    User = value.User;
                    Pass = value.Pass;
                    From = value.From;
                    DisplayName = value.DisplayName;
                    _active = value;
                }
            }
        }

        // 這些是原本 SendAsync 用的屬性
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string User { get; set; } = "";
        public string Pass { get; set; } = "";
        public string From { get; set; } = "";
        public string DisplayName { get; set; } = "Cat Paw Footprint";

        // 🔹 新增兩個切換方法
        public void UsePrimary() => Active = Primary;
        public void UseSecondary() => Active = Secondary;
    }

    public class SmtpAccount
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string User { get; set; } = "";
        public string Pass { get; set; } = "";
        public string From { get; set; } = "";
        public string DisplayName { get; set; } = "Cat Paw Footprint";
    }
}
