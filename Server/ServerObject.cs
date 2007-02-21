using System;
using System.Collections.Generic;
using System.Text;

namespace PractiSES
{
    public class ServerObject : MarshalByRefObject
    {
        public string KeyObt(String email)
        {
            if (email == "cbguder@su.sabanciuniv.edu")
            {
                return "mQGiBEMm9oQRBAC4/Kdg6S/Hw/DLvELu1zuOnLcS/ObTjAZv+YwpYywQ/UNKnN0AKpQR1yWsa/z/oAP/c9d9T57ImsVW42e3xohOuQvTEfX8Ovaz2zhHhhHVnK7e2LTNr5Wu8fj88EyM9CvKvXFcAYVNA4X7aX198BY4VDKAX6IgP9OoPcHD+BGeZwCgoIjsI3yLGBJxmOZ8sAiBWR+gYSMD/A/f7pOa8sYoxSHA9KDiFNPIuvhsP31036eQtTqrHDGXw2vYxQSJeNo86gXYqob6/1JuHvII4rOYHoJNN44wYo5HLm97OFdLz87XY6y7YPxRprq+fI7owglbS0HQhiNfyM08E7xyiI6fa+ekkSjPdUXb62d1R6tEIiRW8OF+Awe0BACSPwzoTZOWG9QeIvPnH8ZElXUKDqURDCL9OAehcFHRmdUzPxrHGzquR7ufrJKzxQPmrmriFfB8mYlxHdeZrndcpgPhpf0hNz6k6CGITnDd5QOiGfRxeBLK3FL+0ay7r9M2YRSPPg1Di44i54/s8ua/yEsI5WSkjAQN3hrWYpdg77Q1Q2FuIEJlcmsgR3VkZXIgKERlamEgVnUpIDxjYmd1ZGVyQHN1LnNhYmFuY2l1bml2LmVkdT6IXgQTEQIAHgUCQyb2hAIbAwYLCQgHAwIDFQIDAxYCAQIeAQIXgAAKCRBxiHK+ZMu2bM4ZAJ9llocFjAq4D9AOZL8Zk5lKZw/R7gCfbzxjFZRYr45frU/v2Vs1NSPcAla5Ag0EQyb2nBAIAIkpGnJzJhtVrhC+908neLOqKwp1WexUeTbElc0Unb0W4MthAIurJ/0AFlhoQGq4sAU5QVX93RpWfL+/WDLaYwm+XJmA7eh6WHnPfnXFCP9hp8G+OC//Ogzq0kwOAsLPGXWqTq75S8po9j744aW/1jKdEzRaSO1KcJiXiQNa7J8SHXvCI3FnKb8NLR/kMDxAFf42N/lYcXy6W4L+37tdeXKArrHLGvRGMBy+VzvCQym8MYD/Ur3Ub/Ps00r1w+9W/iecR82W8C6/0prSmb38WbD8uGi5+m7iSzzV8W2FtoHf5ggHuhjwgPz9ZR7tPsk8p+peRC3+h3AxngMz1+OEvhcAAwUH/3BPidNFBSfMQR3LrMe1ktox+bv9m8XoFpNR4nUgmrx4dcjr2W82uUZmgoDtyi3SWYECjG81HwGx1u5eYpm0tc9RKbL3VqzJ0wa6Ur7NHIjo3MU/HbOek1KwSEf7pFychs+1kqPC9XjNXJqnfS9PsGyaLAvtfodnbtJyhLy1XNQjEBm06IuIcI6EdpGiE8kskdGKh27LJgwAZi0djbMboAOR0r2QU274YPPefTm/Cfw8jwcCYZNZ1GNQQ4jz9L6IlAYC+UBjLEmmqEuTXc56HDo7jm9zwS5UJfabqp240T0Qe06zlwJPnQ8s59mjPcVbwaUn/4DhcfvW9Lj7EFYm+aSISQQYEQIACQUCQyb2nAIbDAAKCRBxiHK+ZMu2bM80AJ9zBTY/BMPu11alsv0Rx5vZS34qNgCeMNEXF4XlGFpklzvaQ1zPrCX/iz0==TUw8";
            }
            else
            {
                return "No public key exists for " + email;
            }
        }

        public void KeyRem(string email)
        {
        }

        public void KeyUpdate(string email, string newKey)
        {
        }

        public void USKeyRem(string email)
        {
        }

        public void USKeyUpdate(string email, string newKey)
        {
        }

    }
}
