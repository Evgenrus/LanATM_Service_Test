using System.Text;
using System.Text.RegularExpressions;
using Delivery.Exceptions;

namespace Delivery.Models;

public class AddressModel
{
    public int Id { get; set; }
    public string Region { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Street { get; set; }
    public string House { get; set; }
    public int? Floor { get; set; }
    public string? Flat { get; set; }
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }

    public static AddressModel FromString(string addr)
    {
        if (!Regex.IsMatch(addr, @"^([a-zа-я-]{3,50},\s){5}\d{1,3}(, \d{1,3}){0,2}$", RegexOptions.IgnoreCase))
        {
            throw new AddressFormatException("Wrong format");
        }

        var items = new string[7];
        var strings = addr.Split(", ");
        for (int i = 0; i < strings.Length; i++)
        {
            items[i] = strings[i];
        }

        return new AddressModel()
        {
            Region = items[0],
            City = items[1],
            District = items[2],
            Street = items[3],
            House = items[4],
            Floor = Convert.ToInt32(items[5]),
            Flat = items[6]
        };
    }

    public override string ToString()
    {
        var sb = new StringBuilder(50);
        sb.AppendFormat($"{Region}, {City}, {District}, {Street}, {House}");
        if (Floor is not null)
            sb.AppendFormat($", {Floor}");
        if (Flat is not null)
            sb.AppendFormat($", {Flat}");

        return sb.ToString();
    }
}