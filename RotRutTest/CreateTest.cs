using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RotRut.Models;
using RotRut;

namespace RotRutTest;

[TestClass]
public class CreateTest
{
    [TestMethod]
    public void MergeDoubleInvoiceNumbersTest()
    {
        List<Payment> payments = new()
        {
            new Payment
            {
                InvoiceNumber = "1000",
                ApprovedAmount = 150
            },
            new Payment
            {
                InvoiceNumber = "1002",
                ApprovedAmount = 200
            },
            new Payment
            {
                InvoiceNumber = "1001",
                ApprovedAmount = 100
            },
            new Payment
            {
                InvoiceNumber = "1002",
                ApprovedAmount = 100
            },
            new Payment
            {
                InvoiceNumber = "1003",
                ApprovedAmount = 175
            },
            new Payment
            {
                InvoiceNumber = "1004",
                ApprovedAmount = 205
            },
            new Payment
            {
                InvoiceNumber = "1004",
                ApprovedAmount = 204
            },
            new Payment
            {
                InvoiceNumber = "1005",
                ApprovedAmount = 350
            }
        };

        var newPayments = payments.MergeDoubleInvoiceNumbers();

        Assert.IsTrue(newPayments.Count() == 6);
    }
}