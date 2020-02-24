using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public enum AlignmentInn
    {
        Classic = 0,
        Good = -1,
        Mediocre = 1
    }

    public enum AlignmentMerchant
    {
        Honest = 0,
        [Description("On Sale")]
        OnSale = -1,
        Fraudulent = 1
    }
}
