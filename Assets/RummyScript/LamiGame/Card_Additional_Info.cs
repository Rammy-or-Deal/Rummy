namespace Assets.RummyScript.LamiGame
{
    public class Card_Additional_Info
    {
        public int jokerCount;
        public int ACount;

        public Card_Additional_Info()
        {

        }
        
        public void SetInfo(string data)
        {
            //data : 0:Joker, 1:A
            //      format: joker:A
            var tmp = data.Split(':');
            jokerCount = int.Parse(tmp[0]);
            ACount = int.Parse(tmp[1]);
        }
    }
}