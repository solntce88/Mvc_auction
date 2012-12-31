using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc_auction.Models
{
    static public class MailChecker
    {
        static public void CheckAndSend()
        {
            lock (typeof(MailChecker)) // блок-ка
            {
                List<Lot> lotList = new LotRepository().GetInactiveLot();
                if (lotList != null)
                {
                    foreach (Lot lot in lotList)
                    {
                        if (lot != null)
                        {

                            if (lot.Customer_id != 0)
                            {
                                User userCustomer = new UserRepository().GetDBUser(lot.Customer_id);
                                MailSender.SendMail(4, userCustomer, lot);
                                User userOwner = new UserRepository().GetDBUser(lot.Owner_id);
                                MailSender.SendMail(6, userOwner, lot);
                            }
                            else
                            {
                                User user = new UserRepository().GetDBUser(lot.Owner_id);
                                MailSender.SendMail(7, user, lot);
                            }
                            //  lot.DateEnd
                        }
                    }
                }
            }
        }
    }
}