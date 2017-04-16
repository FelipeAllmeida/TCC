using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Você quer comprar um bolo, mas não lembra onde fica a confeitaria. Você lembra apenas
 * que, somando o número das casas (1, 2, 3 ...) do início da rua até a confeitaria, e da casa logo após a confeitaria
 * até o fim da rua, ambas as somas são iguais. Infelizmente, você não lembra a rua, e a cidade tem ruas muito
 * longas, com até 400 milhões de casas. Portanto, você precisa descobrir os possíveis números da confeitaria
 * para ruas de tamanho arbitrário (de 1 a 400 milhões). Por exemplo: a confeitaria poderia estar em uma rua de
 * tamanho 8, e seu endereço seria 6 (pois 1+2+3+4+5 = 7+8). Agora só falta encontrar as outras possibilidades
 */

public class GG : MonoBehaviour 
{
    void Start()
    {
        int __maxHouses = 400000000;
        int __candyHouseIndex = -1;

        for (int j = 1;j < __maxHouses;j++)
        {
            int __leftHousesSum = ((j - 1) * j) / 2;
            int __rightHousesSum = ((__maxHouses * (__maxHouses + 1)) / 2) - __leftHousesSum;

            Debug.Log("Current Index: " + j + " | " + __leftHousesSum + " | " + __rightHousesSum);
            if ((__leftHousesSum + j) == __rightHousesSum)
            {
                __candyHouseIndex = j;
                break;
            }
        }
        //for (int j = 1; j < __maxHouses; j++)
        //{
        //    int __leftHousesSum = (j * (j + 1)) / 2;

        //    int __rightHousesSum;
        //    if (__maxHouses % 2 == 0)
        //    {
        //        int __middleHouseSum = ((j + 1) * (j + 2)) / 2;
        //        __rightHousesSum = ((__maxHouses * (__maxHouses + 1)) / 2) - __middleHouseSum;
        //    }
        //    else
        //    {
        //        int __middleHouseSum = (j * (j + 1)) / 2;
        //        __rightHousesSum = ((__maxHouses * (__maxHouses + 1)) / 2) - __middleHouseSum;
        //    }


        //    Debug.Log(__leftHousesSum + " | " + __rightHousesSum);
        //    if (__leftHousesSum == __rightHousesSum)
        //    {
        //        __candyHouseIndex = j + 1;
        //        break;
        //    }

        //}

        Debug.Log("CandyHouseIndex = " + __candyHouseIndex);
    }
}
