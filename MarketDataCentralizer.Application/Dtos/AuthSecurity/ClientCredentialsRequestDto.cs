using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDataCentralizer.Application.Dtos.AuthSecurity
{
    public class ClientCredentialsRequestDto
    {
        public  required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
    }

}
