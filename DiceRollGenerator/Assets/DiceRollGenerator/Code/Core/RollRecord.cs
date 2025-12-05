using System.Collections.Generic;

[System.Serializable]
public class RollRecord {
    public string id;
    public DiceType diceType;
    public int diceCount;
    public List<int> results;
    public int total;
    public System.DateTime timestamp;

    public RollRecord(DiceType type, int count, List<int> res, int tot) {
        id = System.Guid.NewGuid().ToString();
        diceType = type;
        diceCount = count;
        results = new List<int>(res);
        total = tot;
        timestamp = System.DateTime.Now;
    }
}


