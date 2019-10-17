using System;

public class LamiLineCard
{
    public int color;
    public int number;
    public int property;   //PanMgr => 0: normal, 1:last
                           //LamiMe => 0: normal, 1:selected
    int x;
    int y;
    public LamiLineCard(int color, int number)
    {
        this.color = color;
        this.number = number;
        property = 1;
    }
    public string getImage()
    {
        string res = "";
        res = string.Format("new_card/card_{0}_{1}", color, number);
        if (number < 0)
            res = string.Format("new_card/card_{0}_{1}", 0, 0);

        return res;
    }

    public void setProperty(int v)
    {
        property = 0;
    }

}
