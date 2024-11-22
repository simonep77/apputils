// See https://aka.ms/new-console-template for more information
using AppUtils.Lib.Banca;

Console.WriteLine("Hello, World!");


Console.WriteLine("(1) - Test Iban");
var iban = new Iban("IT001234567890000000000000");
Console.WriteLine($"Esito iban: {iban.ValidateFormalNoException()}");
