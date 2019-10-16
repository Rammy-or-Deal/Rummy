namespace Assets.RummyScript.LamiGame
{
    public class LamiMyCard
    {
        public int color;
        public int number;
        private LamiMe lamiMe;

        public bool isSelected; //LamiMe => false: normal, true:selected

        int x;
        int y;

        public LamiMyCard(int color, int number)
        {
            this.color = color;
            this.number = number;
            isSelected = false;
        }

        public string getImage()
        {
            
            string res = "";
            res = string.Format("new_card/card_{0}_{1}", color, number);
            if (number < 0)
                res = string.Format("new_card/card_{0}_{1}", 0, 0);

            return res;
        }

        // If card clicked, call LamiMe.OnSelectedCard() 
        public void OnClick()
        {
            isSelected = (!isSelected);
        }
    }
}