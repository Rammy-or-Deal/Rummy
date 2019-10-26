public enum BuildMethod {
  Product,
  Develop_Message,
  Develop_No_Message
}

public static class Constants{
  public static BuildMethod LamiBuildMethod = BuildMethod.Product;
  public static int FinishDiabledFlag=1;
    public static float turnTime_AutoPlay = 3.0f;
    public static float turnTime_Develop = 1000;
    public static float turnTime_Product = 10;
    public static float waitTime_Develop = 100;
    public static float waitTime_Product = 30;
    public static int botWaitTime = 2;
}