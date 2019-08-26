using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//FIND THE CODE SMELL and REFACTOR 
public interface IPayments
{
    void process();
}
public class CreditCard : IPayments
{
    public void process()
    {
        //Credit Card Processing Logic
    }
}
public class PayPal : IPayments
{
    public void process()
    {
        //Paypal payment Processing Logic
    }
}
public class Google : IPayments
{
    public void process()
    {
        //Google payment Processing Logic
    }
}

public class Amazon : IPayments
{
    public void process()
    {
        //Amazon payment Processing Logic 
    }
}
public class OnlineCart
{
    private IPayments payments;
    public OnlineCart(IPayments payment)
    {
        this.payments = payment;
    }
    public void CheckOut()
    {
        payments.process();
    }
   
}