namespace SharedKernel.Common;

public static class ValidationConstants
{
    /// <summary>
    /// Шаблон валидного номера телефона.
    /// </summary>
    /// <remarks>запретил номера на 8, чтобы не было дублей номеров с +7 и 8</remarks>
    public static readonly string ValidPhoneNumberPattern =
        @"^\+7(?:700|701|702|703|704|705|706|707|708|709|747|750|751|760|761|762|763|764|771|775|776|777|778)\d{7}$";

    /// <summary>
    /// Шаблон валидного цифрового кода.
    /// </summary>
    public static readonly string ValidCodePattern = @"^(\d{4})$";
}
