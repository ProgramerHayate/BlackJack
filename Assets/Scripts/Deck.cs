using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage1;
    public Text probMessage2;
    public Text probMessage3;
    public int[] values = new int[52];
    int cardIndex = 0;
    public Sprite[] randFaces;
    public int[] randValues;
    

    private void Awake()
    {    
        InitCardValues();        

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();        
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        
        int aux = 1;
         for (int i = 0; i < values.Length; i++)
        {
            if (aux > 13)
            {
                aux = 1;
            }
            if (aux > 10)
            {
                values[i] = 10;
                
            }
            else
            {
                values[i] = aux;
                
            }

            if (aux == 1)
            {
                values[i] = 11;
            }
            aux++;
        }
         
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */

        bool contains;
        
        for (int i = 0; i < faces.Length; i++)
        {
            contains = false;
            int aux = Random.Range(0, 52);
            foreach (Sprite a in randFaces)
            {
                if (a.Equals(faces[aux]))
                {
                    contains = true;
                }
            }
            if (!contains)
            {
                randFaces[i] = faces[aux];
                randValues[i] = values[aux];
            }
            else
            {
                i--;
                
                
                
            }
            

        }
        faces = randFaces;
        values = randValues;
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
            CardHand jugador = player.GetComponent<CardHand>();
            CardHand banca = dealer.GetComponent<CardHand>();
            if (jugador.points.Equals(21))
            {
                hitButton.interactable = false;
                stickButton.interactable = false;
                finalMessage.text = "Gana el jugador";
            }
            
            if (banca.points.Equals(21))
            {
                hitButton.interactable = false;
                stickButton.interactable = false;
                finalMessage.text = "Gana la banca";
            }
        }
    }

    private void CalculateProbabilities()
    {
        CardHand jugador = player.GetComponent<CardHand>();
        CardHand banca = dealer.GetComponent<CardHand>();
        float favorables1 = 0;
        float favorables2 = 0;
        float favorables3 = 0;
        int count = 3;
        /* TODO:
          * Calcular las probabilidades de:
          * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador*/
        if (banca.cards.Count.Equals(2)) {
            foreach (int value in values)
            {
                if (banca.cards[1].GetComponent<CardModel>().value + value > jugador.points)
                {
                    if (banca.cards[0].GetComponent<CardModel>().value.Equals(value))
                    {
                        count--;
                    }
                    if(count != 0)
                    {
                        favorables1++;
                    }
                    
                }
            }
        }
        
        probMessage1.text = "Probabilidad de que el dealer tenga mas puntos = " + favorables1 / (52-cardIndex-1);

        // - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
        if (banca.cards.Count.Equals(2))
        {
            foreach (int value in values)
            {
                if (jugador.points + value >= 17 && jugador.points + value <= 21)
                {
                    if (banca.cards[0].GetComponent<CardModel>().value.Equals(value))
                    {
                        count--;
                    }
                    if (count != 0)
                    {
                        favorables2++;
                    }
                }
            }
        }
        probMessage2.text = "Probabilidad de obtener entre 17 y 21 = " + favorables2 / (52 - cardIndex);
        // - Probabilidad de que el jugador obtenga más de 21 si pide una carta 
        if (banca.cards.Count.Equals(2))
        {
            foreach (int value in values)
            {
                if (jugador.points + value > 21)
                {
                    if (banca.cards[0].GetComponent<CardModel>().value.Equals(value))
                    {
                        count--;
                    }
                    if (count != 0)
                    {
                        favorables3++;
                    }
                }
            }
        }   
        probMessage3.text = "Probabilidad de obtener mas de 21 = " + favorables3 / (52 - cardIndex);

    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;
        CalculateProbabilities();
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }       

    public void Hit()
    {
        CardHand jugador = player.GetComponent<CardHand>();
        CardHand banca = dealer.GetComponent<CardHand>();
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        if (cardIndex.Equals(4))
        {
            banca.cards[0].GetComponent<SpriteRenderer>().sprite = banca.cards[0].GetComponent<CardModel>().front;
        }

        //Repartimos carta al jugador

        PushPlayer();
        
        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */
        if (jugador.points > 21)
        {
            hitButton.interactable = false;
            stickButton.interactable = false;
            finalMessage.text = "El jugador a perdido";
        }

    }

    public void Stand()
    {
        CardHand jugador = player.GetComponent<CardHand>();
        CardHand banca = dealer.GetComponent<CardHand>();
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        if (cardIndex.Equals(4))
        {
            banca.cards[0].GetComponent<SpriteRenderer>().sprite = banca.cards[0].GetComponent<CardModel>().front;
        }

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */
         while (banca.points <= 16)
        {
            PushDealer();
        }
        if (jugador.points > banca.points || banca.points > 21)
        {
            hitButton.interactable = false;
            stickButton.interactable = false;
            finalMessage.text = "El jugador a ganado";
        }
        else {
            hitButton.interactable = false;
            stickButton.interactable = false;
            finalMessage.text = "El dealer a ganado";
        }

    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        /*ShuffleCards();
        StartGame();*/
    }
    
}
