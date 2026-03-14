using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Domain.Models.ApiClientSecurity
{
    public class AuthClientSettingsModel
    {
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required string Scope { get; set; }
    }

}
