using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public DeadPile deadPile;
    public BasePlayer[] players = new BasePlayer[2]; 

    public static string FILE_PATH = "NULL!";

    private static Dictionary<int, IPieceBase> pieceCodes = new Dictionary<int, IPieceBase>();

    const char SPLIT_CHAR = '_';

    private void Awake()
    {
        FILE_PATH = Application.persistentDataPath + "/LoadFile.txt";
    }

    public void SaveBoardState()
    {
        if(!pieceCodes.Any())
        {
            CreatePieceDictionary();
        }

        using(StreamWriter sw = File.CreateText(FILE_PATH))
        {
            foreach(var piece in GetAllPieces())
            {
                string pieceCode = piece.GetHashCode().ToString();

                sw.WriteLine(pieceCode);
            }

            sw.Close();
        }
    }

    private void CreatePieceDictionary()
    {
        var pieces = GetAllPieces();

        foreach (var piece in pieces)
        {
            pieceCodes.Add(piece.GetHashCode(), piece);
        }
    }

    private List<IPieceBase> GetAllPieces()
    {
        return new List<IPieceBase>(deadPile.deadPieces.Concat(players[0].GetPieces()).Concat(players[1].GetPieces()));  
    }

    //internal List<IPieceBase> LoadBoardState()
    //{
    //    List<IPieceBase> allPieces = new List<IPieceBase>();

    //    string line = "";

    //    using (StreamReader reader = new StreamReader(FILE_PATH))
    //    {
    //        while ((line = reader.ReadLine()) != null)
    //        {
    //            int key = int.Parse(line);

                

    //        }
    //    }
    //}
}
