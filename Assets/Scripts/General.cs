using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General : MonoBehaviour
{
    enum OyuncuSay�s�
    {
        D�rtOyuncu, // 1 �erif, 1 hain, 2 haydut
        Be�Oyuncu, // 1 �erif, 1 hain, 2 haydut, 1 aynas�z
        Alt�Oyuncu, // 1 �erif, 1 hain, 3 haydut, 1 aynas�z
        YediOyuncu // 1 �erif, 1 hain, 3 haydut, 2 aynas�z
    }
    enum Roller
    {
        �erif, // ama� b�t�n haydutlar� ve haini saf d��� b�rakmak
        Haydut, // ama� �erifi saf d��� b�rakmak, �d�ller i�in di�er haydutlara da sald�rabilir
        Aynas�z, // ama� �erifi korumak
        Hain // ama� oyunda ayakta kalan son oyuncu olmak
    }
}
