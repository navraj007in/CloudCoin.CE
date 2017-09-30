using System;
using System.IO;
using System.Linq;

namespace Founders
{
    public class Frack_Fixer
    {
        /* INSTANCE VARIABLES */
      
        private int totalValueToBank;
        private int totalValueToFractured;
        private int totalValueToCounterfeit;
        private RAIDA raida;


        /* CONSTRUCTORS */
        public Frack_Fixer( int timeout)
        {

            raida = new RAIDA(timeout);
            totalValueToBank = 0;
            totalValueToCounterfeit = 0;
            totalValueToFractured = 0;
        }//constructor

        public string fixOneGuidCorner(int raida_ID, CloudCoin cc, int corner, int[] trustedTriad)
        {
            CoinUtils cu = new CoinUtils(cc);

            /*1. WILL THE BROKEN RAIDA FIX? check to see if it has problems echo, detect, or fix. */
            if (RAIDA_Status.failsFix[raida_ID] || RAIDA_Status.failsEcho[raida_ID] || RAIDA_Status.failsEcho[raida_ID])
            {
          
                return "RAIDA Fails Echo or Fix. Try again when RAIDA online.";
            }
            else
            {
                /*2. ARE ALL TRUSTED RAIDA IN THE CORNER READY TO HELP?*/

                char[] pown_chars = cc.pown.ToCharArray();


                //See if First Trusted RAIDA can help
                if (!pown_chars[trustedTriad[0]].Equals('p'))
                {
                
                    return "RAIDA " + trustedTriad[0] + " can't help on corner  " + corner;
                }
                //See if Second Trusted RAIDA can help
                if ( !pown_chars[trustedTriad[1]].Equals('p') )
                {

                    return "RAIDA " + trustedTriad[1] + " can't help on corner  " + corner;
                }

                //See if Third Trusted RAIDA can help
                if (!pown_chars[trustedTriad[2]].Equals('p'))
                {

                    return "RAIDA " + trustedTriad[2] + " can't help on corner  " + corner;
                }

                if (!RAIDA_Status.failsEcho[trustedTriad[0]] || !RAIDA_Status.failsDetect[trustedTriad[0]] || !RAIDA_Status.failsEcho[trustedTriad[1]] || !RAIDA_Status.failsDetect[trustedTriad[1]] || !RAIDA_Status.failsEcho[trustedTriad[2]] || !RAIDA_Status.failsDetect[trustedTriad[2]])
                {
                    /*3. GET TICKETS AND UPDATE RAIDA STATUS TICKETS*/
                    string[] ans = { cc.an[trustedTriad[0]], cc.an[trustedTriad[1]], cc.an[trustedTriad[2]] };
                    raida.get_Tickets(trustedTriad, ans, cc.nn, cc.sn, cu.getDenomination(), 3000);
                    
                    
                                //Check to see if the coin actully is counterfeits and all the so called "p"s are actually "f"s. 
                    if (raida.responseArray[trustedTriad[0]].fullResponse.Contains("fail") 
                        && raida.responseArray[trustedTriad[1]].fullResponse.Contains("fail")
                        && raida.responseArray[trustedTriad[2]].fullResponse.Contains("fail")
                        ) {
                    
                        return "counterfeit " + corner;

                    }//end if p's are actully fs. 
                    
                    /*4. ARE ALL TICKETS GOOD?*/
                    if (RAIDA_Status.hasTicket[trustedTriad[0]] && RAIDA_Status.hasTicket[trustedTriad[1]] && RAIDA_Status.hasTicket[trustedTriad[2]])
                    {
                        /*5.T YES, so REQUEST FIX*/
                        DetectionAgent da = new DetectionAgent(raida_ID, 5000);
                        Response fixResponse = da.fix(trustedTriad, RAIDA_Status.tickets[trustedTriad[0]], RAIDA_Status.tickets[trustedTriad[1]], RAIDA_Status.tickets[trustedTriad[2]], cc.an[raida_ID]).Result;
                        /*6. DID THE FIX WORK?*/
                        if (fixResponse.success)
                        {
                        
                            return "RAIDA" + raida_ID + " unfracked successfully.";

                        }
                        else
                        {

                            return "RAIDA failed to accept tickets on corner " + corner;
                        }//end if fix respons was success or fail
                    }
                    else
                    {


                        return "  Trusted servers failed to provide tickets for corner " + corner;//no three good tickets
                    }//end if all good
                }//end if trused triad will echo and detect (Detect is used to get ticket)

                return "  One or more of the trusted triad will not echo and detect. So not trying.";
            }//end if RAIDA fails to fix. 

        }//end fix one



        /* PUBLIC METHODS */
    
        // End select all file names in a folder
        public bool deleteCoin(String path)
        {
            bool deleted = false;

            // System.out.println("Deleteing Coin: "+path + this.fileName + extension);
            try
            {
    //            File.Delete(path);
            }
            catch (Exception e)
            {
      //          Console.WriteLine(e);
        //        CoreLogger.Log(e.ToString());
            }
            return deleted;
        }//end delete coin


        public CoinUtils fixCoin(CloudCoin brokeCoin)
        {
            CoinUtils cu = new CoinUtils(brokeCoin);

            /*0. RESET TICKETS IN RAIDA STATUS TO EMPTY*/
            RAIDA_Status.resetTickets();
            /*0. RESET THE DETECTION to TRUE if it is a new COIN */
            RAIDA_Status.newCoin();

            cu.setAnsToPans();// Make sure we set the RAIDA to the cc ans and not new pans. 
            DateTime before = DateTime.Now;

            String fix_result = "";
            FixitHelper fixer;

            /*START*/
            /*1. PICK THE CORNER TO USE TO TRY TO FIX */
            int corner = 1;
            // For every guid, check to see if it is fractured
            for (int raida_ID = 0; raida_ID < 25; raida_ID++)
            {
                //Check to see if the coin has been marked counterfeit and should be moved to trash
               // Console.Out.WriteLine("The folder is " + cu.getFolder().ToLower());
                if (cu.cc.pown == "fffffffffffffffffffffffff") {
                    cu.setFolder("counterfeit");
                    return cu; }
                
                //  Console.WriteLine("Past Status for " + raida_ID + ", " + brokeCoin.pastStatus[raida_ID]);

                if (cu.getPastStatus(raida_ID).ToLower() != "pass")//will try to fix everything that is not perfect pass.
                {
                    cu.cc.an[raida_ID] = cu.generatePan();//Assign the AN a new PAN for security. 
        

                    fixer = new FixitHelper(raida_ID, brokeCoin.an.ToArray());

                    //trustedServerAns = new String[] { brokeCoin.ans[fixer.currentTriad[0]], brokeCoin.ans[fixer.currentTriad[1]], brokeCoin.ans[fixer.currentTriad[2]] };
                    corner = 1;
                    while (!fixer.finnished)
                    {
                    
                        fix_result = fixOneGuidCorner(raida_ID, brokeCoin, corner, fixer.currentTriad);
                        // Console.WriteLine(" fix_result: " + fix_result + " for corner " + corner);
                        if (fix_result.Contains("success"))
                        {
                            //Fixed. Do the fixed stuff
                            cu.setPastStatus("pass", raida_ID);
                            fixer.finnished = true;
                            corner = 1;
                        }
                        else if (fix_result.Contains("counterfeit"))
                        {
                            for (int j = 0; j < 25; j++) { cu.setPastStatus("fail", j); }//Set all status to fail so the coin will be moved to counterfeit. 
                            cu.setFolder("counterfeit");
                            fixer.finnished = true;
                            return cu;
                        }
                        else
                        {
                            //Still broken, do the broken stuff. 
                            corner++;
                            fixer.setCornerToCheck(corner);
                        }
                    }//End whild fixer not finnished
                }//end if RAIDA past status is passed and does not need to be fixed
            }//end for each AN

            for (int raida_ID = 24; raida_ID > -1; raida_ID--)
            {
                //  Console.WriteLine("Past Status for " + raida_ID + ", " + brokeCoin.pastStatus[raida_ID]);
               // Console.WriteLine("Pown is " + cu.cc.pown);
                if (cu.getPastStatus(raida_ID).ToLower() != "pass")//will try to fix everything that is not perfect pass.
                {
                    cu.cc.an[raida_ID] = cu.generatePan();//Assign the AN a new PAN for security. 
                   


                    fixer = new FixitHelper(raida_ID, brokeCoin.an.ToArray());

                    //trustedServerAns = new String[] { brokeCoin.ans[fixer.currentTriad[0]], brokeCoin.ans[fixer.currentTriad[1]], brokeCoin.ans[fixer.currentTriad[2]] };
                    corner = 1;
                    while (!fixer.finnished)
                    {
                    
                        fix_result = fixOneGuidCorner(raida_ID, brokeCoin, corner, fixer.currentTriad);
                        // Console.WriteLine(" fix_result: " + fix_result + " for corner " + corner);
                        if (fix_result.Contains("success"))
                        {
                            //Fixed. Do the fixed stuff
                            cu.setPastStatus("pass", raida_ID);
                            fixer.finnished = true;
                            corner = 1;
                        }
                         else if (fix_result.Contains("counterfeit"))
                        {
                            for (int j = 0; j < 25; j++) { cu.setPastStatus("fail", j); }//Set all status to fail so the coin will be moved to counterfeit. 
                            cu.setFolder("counterfeit");
                            fixer.finnished = true;
                            return cu;
                        }
                        else
                        {
                            //Still broken, do the broken stuff. 
                            corner++;
                            fixer.setCornerToCheck(corner);
                        }
                    }//End whild fixer not finnished
                }//end if RAIDA past status is passed and does not need to be fixed
            }//end for each AN
            DateTime after = DateTime.Now;
            TimeSpan ts = after.Subtract(before);
    

            cu.calculateHP();//how many fails did it get
                             //  cu.gradeCoin();// sets the grade and figures out what the file extension should be (bank, fracked, counterfeit, lost

            cu.grade();
            cu.calcExpirationDate();
            return cu;
        }// end fix coin

    }//end class
}//end namespace

