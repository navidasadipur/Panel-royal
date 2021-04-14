using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpadStorePanel.Core.Utility
{
    public enum DiscountType
    {
        Percentage = 1,
        Amount = 2
    }
    public enum GeoDivisionType
    {
        Country = 0,
        State = 1,
        City = 2,
    }

    public enum ProductFeatures
    {
        Color = 1002,
        Size = 1003,
    }

    public enum StaticContents
    {
        Phone = 1005,
        Map = 2005,
        Address = 6,
        WorkingHours = 1008,
        Email = 2,
        Youtube = 2007,
        Instagram = 2012,
        Twitter = 2013,
        Pinterest = 2014,
        Facebook = 2015,
        BlogImage = 1013,
        ContactInfo = 1014,
        companyServices = 3003,
        CopyRight = 3004,
        ImplementaitonService = 3005,
        
        Logo = 3,
        BackgroundImage = 4022
    }

    public enum StaticContentTypes
    {
        About = 1,
        Socials = 2,
        InstagramImages = 3,

        HomeTopSlider = 11,
        CompanyHistory = 2,
        BlogImage = 1004,
        
        OurServices = 10,
    }
}
