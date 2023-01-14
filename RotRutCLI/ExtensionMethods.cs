using RotRut.Models;

namespace RotRut;

public static class ExtensionMethods
{
    public static IEnumerable<Payment> MergeDoubleInvoiceNumbers(
        this IEnumerable<Payment> payments) =>
            payments.GroupBy(p => p.InvoiceNumber)
                .Select(x => new Payment
                {
                    InvoiceNumber = x.Key,
                    ApprovedAmount = x.Sum(p => p.ApprovedAmount)
                });

    public static bool ContainsDoubleInvoiceNumbers(this IEnumerable<Payment> payments) =>
        payments.Any(item => !new HashSet<Payment>().Add(item));
}
