namespace Assets.RummyScript.LamiGame
{
    public class LamiUser_Info
    {
        public int id;
        public string name;
        public string picUrl;
        public int coinValue;
        public string skillLevel;
        public int  frameId;

        public LamiUser_Info()
        {

        }
        public void SetInfo(string data)
        {
            //data : 0:id, 1:name, 2:picUrl, 3:coinValue, 4:skillLevel, 5:framePicUrl
            //      format: id:name:picUrl:coinValue:skillLevel:framePic
            var tmp = data.Split(':');
            id = int.Parse(tmp[0]);
            name = tmp[1];
            picUrl = tmp[2];
            coinValue = int.Parse(tmp[3]);
            skillLevel = tmp[4];
            frameId = int.Parse(tmp[5]);
        }
    }
}