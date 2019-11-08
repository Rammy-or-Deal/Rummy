using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static class FortuneRuleMgr
{
    // Start is called before the first frame update
    public static int handSuitNo;
    public static int luckyNo;
    public static int highCard;

    public static HandSuit GetCardType(List<Card> list, ref List<Card> resList)
    {
        CardData cardData = new CardData();
        List<byte> m_Cards = new List<byte>();
        for (int i = 0; i < list.Count; i++)
        {
            m_Cards.Add(new Card(list[i].num, list[i].color).byteValue);
        }
        byte[] resCard = new byte[m_Cards.Count];
        var t = cardData.GetScore(m_Cards.ToArray(), ref resCard);

        resList.Clear();
        for (int i = 0; i < resCard.Length; i++)
        {
            Card card = new Card();
            card.byteValue = resCard[i];
            resList.Add(card);
        }
        return (HandSuit)(CardData.GetScoreToCardType(t));
    }
    public static int GetScore(List<Card> list, HandSuit type)
    {
        int score = 0;
        score += ((int)type+1) * 2000;
        score += list[0].num * 100;
        score += (4-list[0].color)*10;

        return score;
    }
    public static string GetCardTypeString(HandSuit type)
    {
        string res = "";
        res = type.ToString();
        res = res.Replace('_', ' ');
        return res;
    }
}


/// <summary>
/// 16진수로
/// 십단위는 카드 문양
/// 0x10 클로버
/// 0x20 다이아
/// 0x30 하트
/// 0x40 스페이드
/// 일단위는 숫자
/// 0x01 에이스 (스트레이트에서만)
/// 0x02 2번카드
/// 0x03 3번카드
/// 0x04 4번카드
/// 0x05 5번카드
/// 0x06 6번카드
/// 0x07 7번카드
/// 0x08 8번카드
/// 0x09 9번카드
/// 0x0A 10번카드
/// 0x0B J카드
/// 0x0C Q카드
/// 0x0D K카드
/// 0x0E A카드
/// 
/// 예제
/// 0x11 클로버 에이스카드
/// 0x3A 하트 10번카드
/// 
/// 16진수로 점수 매김
/// 0x09000000 로얄스트레이트 플러쉬
/// 0x08000000 스트레이트플러쉬
/// 0x07000000 포카드
/// 0x06000000 풀하우스
/// 0x05000000 플러쉬
/// 0x04000000 스트레이트
/// 0x03000000 트리플
/// 0x02000000 투페어
/// 0x01000000 원페어
/// 0x00000000 하이카드
/// 
/// </summary>

public class CardData
{
    public class CDSub
    {
        public byte[] cards;
        public CardData_Design design = null;
        public CardData_Number number = null;


        void Alloc()
        {
            design = new CardData_Design();
            design.Alloc();
            number = new CardData_Number();
            number.Alloc();
        }

        void AllSort()
        {
            design.Sort();
            number.Sort();
        }

        public void Set(byte[] card)
        {
            List<byte> init = new List<byte>();
            init.AddRange(card);
            init.Sort();
            Alloc();

            cards = init.ToArray();
            int i, j;
            j = cards.Length;
            for (i = 0; i < j; i++)
            {
                design.Add(cards[i]);
                number.Add(cards[i]);
            }
            AllSort();
        }

        public int CheckRoyalStraightFlush(ref byte[] UseCard)
        {
            return design.CheckRoyalStraightFlush(ref UseCard);
        }

        public int CheckStraightFlush(ref byte[] UseCard)
        {
            return design.CheckStraightFlush(ref UseCard);
        }

        public int CheckFourCard(ref byte[] UseCard)
        {
            return number.CheckFourCard(ref UseCard);
        }

        public int CheckFullHouse(ref byte[] UseCard)
        {
            return number.CheckFullHouse(ref UseCard);
        }

        public int CheckFlush(ref byte[] UseCard)
        {
            return design.CheckFlush(ref UseCard);
        }

        public int CheckStraight(ref byte[] UseCard)
        {
            return number.CheckStraight(ref UseCard);
        }

        public int CheckTriple(ref byte[] UseCard)
        {
            return number.CheckTriple(ref UseCard);
        }

        public int CheckTwoPair(ref byte[] UseCard)
        {
            return number.CheckTwoPair(ref UseCard);
        }

        public int CheckOnePair(ref byte[] UseCard)
        {
            return number.CheckOnePair(ref UseCard);
        }

        public int CheckHighCard(ref byte[] UseCard)
        {
            return number.CheckHighCard(ref UseCard);
        }
    }

    public enum CardScoreType
    {
        RoyalStraightFlush,
        StraightFlush,
        FourCard,
        FullHouse,
        Flush,
        Straight,
        Triple,
        TwoPair,
        OnePair,
        HighCard,
        Error
    }


    public static byte[] sDefaultCard = new byte[52] {
        0x12,0x13,0x14,0x15,0x16,0x17,0x18,0x19,0x1A,0x1B,0x1C,0x1D,0x1E,
        0x22,0x23,0x24,0x25,0x26,0x27,0x28,0x29,0x2A,0x2B,0x2C,0x2D,0x2E,
        0x32,0x33,0x34,0x35,0x36,0x37,0x38,0x39,0x3A,0x3B,0x3C,0x3D,0x3E,
        0x42,0x43,0x44,0x45,0x46,0x47,0x48,0x49,0x4A,0x4B,0x4C,0x4D,0x4E,
    };

    public static byte[] GetSuffleCard()
    {
        List<byte> d = new List<byte>();
        List<byte> s = new List<byte>();
        d.AddRange(sDefaultCard);
        Random r = new Random();
        int i;
        for (i = 0; i < 52; i++)
        {
            int n = r.Next(0, 52 - i);
            s.Add(d[n]);
            d.RemoveAt(n);
        }

        return s.ToArray();
    }

    public static byte[] Shuffle(byte[] arr)
    {
        List<byte> d = new List<byte>();
        List<byte> s = new List<byte>();
        d.AddRange(arr);
        Random r = new Random();
        int i, j;
        j = d.Count;
        for (i = 0; i < j; i++)
        {
            int n = r.Next(0, j - i);
            s.Add(d[n]);
            d.RemoveAt(n);
        }
        return s.ToArray();
    }

    //Card Data 

    public List<byte> m_Cards = new List<byte>();


    public void InitCardData()
    {
        byte[] card = GetSuffleCard();
        m_Cards.AddRange(card);
    }

    public byte[] PopCard(int cou)
    {
        List<byte> r = new List<byte>();
        for (int i = 0; i < cou; i++)
        {
            r.Add(m_Cards[0]);
            m_Cards.RemoveAt(0);
        }
        return r.ToArray();
    }

    public int GetScore(byte[] card, ref byte[] useCard)
    {
        if (card == null || card.Length == 0)
            return 0;

        CDSub d = new CDSub();
        d.Set(card);
        int s;
        s = d.CheckRoyalStraightFlush(ref useCard);//고정점수
        if (s > 0) return s;
        s = d.CheckStraightFlush(ref useCard);
        if (s > 0) return s;
        s = d.CheckFourCard(ref useCard);
        if (s > 0) return s;
        s = d.CheckFullHouse(ref useCard);
        if (s > 0) return s;
        s = d.CheckFlush(ref useCard);
        if (s > 0) return s;
        s = d.CheckStraight(ref useCard);//끝자리가 제일 큰 숫자
        if (s > 0) return s;
        s = d.CheckTriple(ref useCard);//끝자리가 3장 숫자
        if (s > 0) return s;
        s = d.CheckTwoPair(ref useCard);
        if (s > 0) return s;
        s = d.CheckOnePair(ref useCard);
        if (s > 0) return s;
        return d.CheckHighCard(ref useCard);
    }

    public string GetScoreString(byte[] card, ref byte[] useCard)
    {
        if (card == null || card.Length == 0)
            return "";

        CDSub d = new CDSub();
        d.Set(card);
        int s;
        s = d.CheckRoyalStraightFlush(ref useCard);//고정점수
        if (s > 0) return "Royal Flush";
        s = d.CheckStraightFlush(ref useCard);
        if (s > 0) return "Straight Flush";
        s = d.CheckFourCard(ref useCard);
        if (s > 0) return "Four Of A Kind";
        s = d.CheckFullHouse(ref useCard);
        if (s > 0) return "Full House";
        s = d.CheckFlush(ref useCard);
        if (s > 0) return "Flush";
        s = d.CheckStraight(ref useCard);//끝자리가 제일 큰 숫자
        if (s > 0) return "Straight";
        s = d.CheckTriple(ref useCard);//끝자리가 3장 숫자
        if (s > 0) return "Tripple";
        s = d.CheckTwoPair(ref useCard);
        if (s > 0) return "Two Pair";
        s = d.CheckOnePair(ref useCard);
        if (s > 0) return "Pair";

        return "High Card";
    }

    public static HandSuit GetScoreToCardType(int score)
    {
        int t = (score & 0x0F000000) >> 24;
        /// 0x09000000 로얄스트레이트 플러쉬
        /// 0x08000000 스트레이트플러쉬
        /// 0x07000000 포카드
        /// 0x06000000 풀하우스
        /// 0x05000000 플러쉬
        /// 0x04000000 스트레이트
        /// 0x03000000 트리플
        /// 0x02000000 투페어
        /// 0x01000000 원페어
        /// 0x00000000 하이카드
        switch (t)
        {
            case 9: return HandSuit.Royal_Flush;
            case 8: return HandSuit.Straight_Flush;
            case 7: return HandSuit.Four_Of_A_Kind;
            case 6: return HandSuit.Full_House;
            case 5: return HandSuit.Flush;
            case 4: return HandSuit.Straight;
            case 3: return HandSuit.Triple;
            case 2: return HandSuit.Two_Pair;
            case 1: return HandSuit.Pair;
            case 0: return HandSuit.High_Card;
        }
        return HandSuit.Error;
    }

    public int TestCompare(byte[] card1, byte[] card2)
    {
        /*int s1 = GetScore(card1);
        int s2 = GetScore(card2);
        if (s1 == s2)
            return 0;
        if (s1 > s2)
            return -1;*/
        return 1;
    }

}

public class CardData_Design
{
    CardData_DesignPart[] m_Design;
    public void Alloc()
    {
        m_Design = new CardData_DesignPart[4];
        for (int i = 0; i < 4; i++)
            m_Design[i] = new CardData_DesignPart();
    }

    public void Add(byte card)
    {
        byte t = (byte)((card & 0xF0) >> 4);
        if ((t - 1) < 0 || (t - 1) > 3)
            return;
        m_Design[t - 1].card.Add(card);
    }

    public void Sort()
    {
        for (int i = 0; i < 4; i++)
            m_Design[i].card.Sort();
    }

    public int CheckRoyalStraightFlush(ref byte[] UseCard)
    {
        //byte[] arr = { 0x01, 0x0A, 0x0B, 0x0C, 0x0D, };
        byte[] arr = { 0x0A, 0x0B, 0x0C, 0x0D, 0x0E };
        for (int i = 0; i < 4; i++)
        {
            if (m_Design[i].CheckDesign(arr))
            {
                Array.Copy(arr, UseCard, 5);
                for (int ii = 0; ii < 5; ii++)
                {
                    UseCard[ii] |= (byte)((i + 1) << 4);
                }
                return 0x09000000;
            }
        }
        return 0;
    }

    public int CheckStraightFlush(ref byte[] UseCard)
    {
        for (int i = 0; i < 4; i++)
        {
            int s = m_Design[i].CheckStraightFlush(ref UseCard);
            if (s > 0)
            {
                for (int ii = 0; ii < 5; ii++)
                {
                    UseCard[ii] |= (byte)((i + 1) << 4);
                }
                return s;
            }
        }
        return 0;
    }

    public int CheckFlush(ref byte[] UseCard)
    {
        int s = 0;
        for (int i = 0; i < 4; i++)
        {
            s = m_Design[i].CheckFlush(ref UseCard);
            if (s > 0)
                return s;
        }
        return 0;
    }
}
public class CardData_Number
{
    public List<byte>[] m_Card = null;
    //public byte[] m_NumberCounter = null;   

    public void Alloc()
    {
        m_Card = new List<byte>[14];
        //m_NumberCounter = new byte[13];
        for (int i = 0; i < 14; i++)
        {
            m_Card[i] = new List<byte>();
            //m_NumberCounter[i] = 0;
        }

    }

    public void Add(byte card)
    {
        byte t = (byte)(card & 0x0F);
        m_Card[t - 1].Add(card);
        if (t == 14)
        {
            //제일 높은 a카드가 나올경우 제일낮은 a카드에도 값을 넣어줌
            byte tt = (byte)((t & 0xF0) | 0x01);
            m_Card[0].Add(tt);
        }
        //m_NumberCounter[t - 1]++;
    }

    public void Sort()
    {
        for (int i = 0; i < 14; i++)
            m_Card[i].Sort();
    }

    public int CheckFourCard(ref byte[] UseCard)
    {
        //포카드가 발견되면 점수에 포함하고 제일 높은 카드를 추가한다.
        //1번배열은 제외한다.
        int s = 0;
        int ls = 0;
        int i;
        for (i = 13; i > 0; i--)
        {
            if (m_Card[i].Count == 4)
            {
                s = i;
                break;
                //return 0x07000000 + i;
            }
        }
        if (s == 0)
            return 0;
        for (i = 13; i > 0; i--)
        {
            if (m_Card[i].Count > 0)
            {
                if (i == s)
                    continue;
                ls = i;
                break;
            }
        }

        s += 1;
        byte[] outArr = new byte[4] { 0x10, 0x20, 0x30, 0x40 };
        Array.Copy(outArr, UseCard, 4);
        for (i = 0; i < 4; i++)
            UseCard[i] |= (byte)s;
        UseCard[4] = m_Card[ls][0];
        ls += 1;
        return 0x07000000 | (s << 4) | ls;
    }

    public int CheckCardCount(int count, ref byte[] UseCard)
    {
        //갯수 에 맞는 카드를 찾는다 1번배열은 제외한다.
        for (int i = 13; i > 0; i--)
        {
            if (m_Card[i].Count == count)
            {
                for (int ii = 0; ii < m_Card[i].Count; ii++)
                {
                    UseCard[ii] = m_Card[i][ii];
                }
                return i + 1;
            }
        }
        return 0;
    }

    public int CheckFullHouse(ref byte[] UseCard)
    {
        int n1, n2;
        byte[] tree = new byte[3];
        byte[] two = new byte[2];
        //풀하우스는 3개짜리 카드가 높은자리 2개짜리 카드가 낮은자리(제일편함)
        n1 = CheckCardCount(3, ref tree);
        if (n1 == 0)
            return 0;
        n2 = CheckCardCount(2, ref two);
        if (n2 == 0)
            return 0;

        Array.Copy(tree, 0, UseCard, 0, 3);
        Array.Copy(two, 0, UseCard, 3, 2);

        n1 = (n1 << 8) | n2;

        return 0x06000000 | n1;
    }

    public int CheckStraight(ref byte[] UseCard)
    {
        int i;
        int c = 0;
        //스트레이트는 순차적이기때문에 제일 큰 숫자만 점수에 포함
        //a2345 를 위해서 1번배열 사용
        for (i = 13; i >= 0; i--)
        {
            if (m_Card[i].Count > 0)
            {
                c++;
                if (c == 5)
                {
                    for (int ii = 0; ii < 5; ii++)
                    {
                        UseCard[ii] = m_Card[i + (4 - ii)][0];
                    }
                    return 0x04000000 | (i + 5);
                }
            }
            else
            {
                c = 0;
            }
        }
        return 0;
    }

    public int CheckTriple(ref byte[] UseCard)
    {
        //3장의 카드가 높으면 이기고
        //같을경우 나머지 두장의 카드로 우위를 정한다.
        //0번배열은 안씀
        int i;
        int s = 0;
        int c = -1;
        //3장의 점수를 3번째로
        for (i = 13; i > 0; i--)
        {
            if (m_Card[i].Count == 3)
            {
                Array.Copy(m_Card[i].ToArray(), UseCard, 3);
                c = i;
                s = i + 1;
                break;
            }
        }
        if (s == 0)
            return 0;

        //탑을 두번째 그다음을 가장 낮은 숫자로 한다. 
        int n = 0;
        for (i = 13; i > 0; i--)
        {
            if (i == c)
                continue;
            if (m_Card[i].Count > 0)
            {
                UseCard[3 + n] = m_Card[i][0];
                s = s << 4;
                s |= i + 1;
                n++;
                if (n == 2)
                    break;
            }
        }
        return 0x03000000 | s;
    }

    public int CheckTwoPair(ref byte[] UseCard)
    {
        int i;
        int s = 0;
        List<int> find = new List<int>();
        //제일 높은카드를 두장짜리 찾은다음 또 그다음 두장짜리를 찾고 한장을 찾는다. 
        //0번배열은 안씀
        for (i = 13; i > 0; i--)
        {
            if (m_Card[i].Count == 2)
            {
                find.Add(i);
                s = s << 4;
                s |= i + 1;
                if (find.Count == 2)
                    break;
            }
        }
        if (find.Count < 2)//두개가 안되면 실패
            return 0;
        Array.Copy(m_Card[find[0]].ToArray(), UseCard, 2);
        Array.Copy(m_Card[find[1]].ToArray(), 0, UseCard, 2, 2);
        //한장짜리중 제일 높은 숫자한개를 찾는다.
        for (i = 13; i > 0; i--)
        {
            if (find[0] == i || find[1] == i)
                continue;
            if (m_Card[i].Count > 0)
            {
                UseCard[4] = m_Card[i][0];
                s = s << 4;
                s |= i + 1;
                break;
            }
        }

        return 0x02000000 | s;
    }

    public int CheckOnePair(ref byte[] UseCard)
    {
        //2장의 카드를 찾고 높은 순서대로 한장씩 찾는다.
        //2장이상이 나오면 더이상 원페어가 아니기때문에 비교할필요는 없다.
        //0번배열은 안씀
        int i;
        int s = 0;
        //3장의 점수를 3번째로
        for (i = 13; i > 0; i--)
        {
            if (m_Card[i].Count == 2)
            {
                UseCard[0] = m_Card[i][0];
                UseCard[1] = m_Card[i][1];
                s = i + 1;
                break;
            }
        }
        if (s == 0)
            return 0;

        //한개짜리 숫자중 제일 높은거 3개를 뽑는다.
        int n = 0;
        for (i = 13; i > 0; i--)
        {
            if (m_Card[i].Count == 1)
            {
                UseCard[n + 2] = m_Card[i][0];
                s = s << 4;
                s |= i + 1;
                n++;
                if (n == 3)
                    break;
            }
        }
        return 0x01000000 | s;
    }

    public int CheckHighCard(ref byte[] UseCard)
    {
        //한개짜리 숫자중 제일 높은거 5개를 뽑는다.
        int n = 0;
        int i;
        int s = 0;
        for (i = 13; i > 0; i--)
        {
            if (m_Card[i].Count == 1)
            {
                UseCard[n] = m_Card[i][0];
                s = s << 4;
                s |= i + 1;
                n++;
                if (n == 5)
                    break;
            }
        }
        return s;
    }
}
public class CardData_DesignPart
{
    public List<byte> card = new List<byte>();
    public int Count { get { return card.Count; } }

    public int CheckDesign(byte d)
    {
        d = (byte)(d & 0x0F);
        int i, j;
        j = card.Count;
        for (i = 0; i < j; i++)
            if ((byte)(card[i] & 0x0F) == d)
                return i;
        return -1;
    }

    public bool CheckDesign(byte[] d)
    {
        int i, j;
        j = d.Length;
        if (card.Count < j)
            return false;
        for (i = 0; i < j; i++)
        {
            int c = CheckDesign(d[i]);
            if (c == -1)
                return false;
        }
        return true;
    }

    public int CheckStraightFlush(ref byte[] UseCard)
    {
        byte[] arr = { 0x0E, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E };
        byte[] comarr = new byte[5];
        for (int i = 9; i >= 0; i--)
        {
            Array.Copy(arr, i, comarr, 0, 5);
            if (CheckDesign(comarr) == true)
            {
                Array.Copy(comarr, UseCard, 5);
                return 0x08000000 + (i + 5);
            }
        }
        return 0;

    }

    public int CheckFlush(ref byte[] UseCard)
    {
        int i, j;
        j = card.Count;
        if (j < 5)
            return 0;
        int v = 0;
        for (i = 0; i < 5; i++)
        {
            int n = card.Count - (i + 1);
            v = v << 4;
            v |= card[n] & 0x0f;
            UseCard[i] = card[n];
        }

        return 0x05000000 | v;
    }
}