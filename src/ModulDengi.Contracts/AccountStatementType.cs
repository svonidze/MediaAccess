namespace ModulDengi.Contracts
{
    using System.Xml.Serialization;

    public enum AccountStatementType
    {
        Undefined = 0,

        /// <summary>
        /// Отмена сбора средств проекта 
        /// </summary>
        [XmlEnum("unknown")]
        Unknown,

        /// <summary>
        /// Пополнение (счет)
        /// </summary>
        [XmlEnum("income")]
        Income,

        /// <summary>
        /// Вывод средств
        /// </summary>
        [XmlEnum("outcome")]
        Outcome,

        /// <summary>
        /// Выдача займа
        /// </summary>
        [XmlEnum("fund")]
        Fund,

        /// <summary>
        /// Возврат займа
        /// </summary>
        [XmlEnum("refundMain")]
        RefundMain,

        /// <summary>
        /// Комиссия по цессии
        /// </summary>
        [XmlEnum("cessionCommission")]
        CessionCommission,

        /// <summary>
        /// Получение процентов по займу (срочные)
        /// Получение процентов по займу (за основной срок)
        /// </summary>
        [XmlEnum("refundPercentage")]
        RefundPercentage,

        /// <summary>
        /// Получение процентов по займу (штрафные сверхсрочные)
        /// </summary>
        [XmlEnum("refundPenaltyPercentage")]
        RefundPenaltyPercentage,
    }
}