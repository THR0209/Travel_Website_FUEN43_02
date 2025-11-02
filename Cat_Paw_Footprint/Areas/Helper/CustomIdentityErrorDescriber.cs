using Microsoft.AspNetCore.Identity;

public class CustomIdentityErrorDescriber : IdentityErrorDescriber
{
	public override IdentityError PasswordTooShort(int length)
		=> new IdentityError { Code = nameof(PasswordTooShort), Description = $"密碼長度至少需 {length} 個字元。" };

	public override IdentityError PasswordRequiresNonAlphanumeric()
		=> new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "密碼需包含至少一個特殊符號（如 !@#$）。" };

	public override IdentityError PasswordRequiresDigit()
		=> new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "密碼需包含至少一個數字（0-9）。" };

	public override IdentityError PasswordRequiresLower()
		=> new IdentityError { Code = nameof(PasswordRequiresLower), Description = "密碼需包含至少一個小寫字母（a-z）。" };

	public override IdentityError PasswordRequiresUpper()
		=> new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "密碼需包含至少一個大寫字母（A-Z）。" };

	public override IdentityError DuplicateUserName(string userName)
		=> new IdentityError { Code = nameof(DuplicateUserName), Description = $"帳號「{userName}」已存在。" };

	public override IdentityError DuplicateEmail(string email)
		=> new IdentityError { Code = nameof(DuplicateEmail), Description = $"電子郵件「{email}」已被使用。" };
}