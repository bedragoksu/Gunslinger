using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General : MonoBehaviour
{
    enum OyuncuSayýsý
    {
        DörtOyuncu, // 1 þerif, 1 hain, 2 haydut
        BeþOyuncu, // 1 þerif, 1 hain, 2 haydut, 1 aynasýz
        AltýOyuncu, // 1 þerif, 1 hain, 3 haydut, 1 aynasýz
        YediOyuncu // 1 þerif, 1 hain, 3 haydut, 2 aynasýz
    }
    enum Roller
    {
        Þerif, // amaç bütün haydutlarý ve haini saf dýþý býrakmak
        Haydut, // amaç þerifi saf dýþý býrakmak, ödüller için diðer haydutlara da saldýrabilir
        Aynasýz, // amaç þerifi korumak
        Hain // amaç oyunda ayakta kalan son oyuncu olmak
    }
}
