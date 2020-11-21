using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameControllerSingle : MonoBehaviour
{
    /* Players e componentes de seu layout */
    public PlayerBaseSingle player1;
    public Text pointPlayer;
    public Text pointCpu;
    public TextMeshProUGUI textCpuLife;

    // Variáveis de controle da cpu
    public PlayerBaseSingle cpu;
    private Computer computer;
    public int nivel;
    public int estilo;
    private int auxEstilo;

    // Turno atual
    public int currentTurn = 1;


    // Controle de rodada
    private bool isBattle = false;
    private bool endTurn = false;

    // Controle de tempo
    private float time;

    // Controle do vencedor
    private string vencedor;
    public GameObject panelVencedor;
    public int faseAtual;

    // Controle da partida
    public static GameControllerSingle instance;
    private bool controlRank;

    // Start is called before the first frame update
    void Start()
    {
        // inicializando os componentes necessários para o game
        instance = this;
        player1.jogadaVez = true;
        cpu.jogadaVez = false;
        controlRank = false;


        time = 0;
        computer = new Computer(player1, cpu);

        if(estilo == 4)
        {
            auxEstilo = UnityEngine.Random.Range(1, 3);
        }
    }

    private void Update()
    {
        // atribuindo a vida da cpu ao placar do player
        textCpuLife.text = cpu.getLife.ToString();

        /* Resposta da Cpu, escolhido a partir do nível
         * Só vai ser verdadeiro quando não for a vez do player,
         * não estar em batalha e o não for o fim do turno.
         * */
        if (!player1.jogadaVez && !isBattle && !endTurn)
        {
            switch (nivel)
            {
                case 1:
                    if(computer.cpuResposta())
                    {
                        isBattle = true;
                    }
                break;

                case 2:
                    if(computer.cpuRespostaMedium(estilo))
                    {
                        isBattle = true;
                    }
                break;

                case 3:
                    if(computer.cpuRespostaHard(estilo))
                    {
                        isBattle = true;
                    }
                break;

                case 4:
                    if(computer.cpuRespostaHard(auxEstilo))
                    {
                        isBattle = true;
                    }
                break;
            }
        }

        pointPlayer.text = player1.pontos.ToString();
        pointCpu.text = cpu.pontos.ToString();


        // Critério para reinicio do jogo, mana vazia adversário recebe ponto
        if (cpu.getMana <= 0)
        {
            Reiniciar();
            Debug.Log("Mana cpu zerada");
        }
        
        // Quando estiver autorizado a batalha, começa a contagem
        if(isBattle)
        {
            time += Time.deltaTime;
            player1.ControleBotoes(false);
        }

        // Contagem para começar a batalha das cartas
        if (time >= 3)
        {
            Battle();
            time = 0;
            if (!player1.SetInit)
            {
                player1.ControleBotoes(true);
            }
        }

        // Termina o jogo quando chegar no 3 round ou vida de algum dos players for 0
        FimJogo();
    }

    // Player sem mana critério que entrega ponto ao adversário e restaura sua mana
    public void PlayerSemMana()
    {
        if (player1.SemMana)
        {
            ManaZeroReiniciar(cpu);
            cpu.RestaurarMana();
        }
    }

    private List<string> SelectBattleCards(PlayerBaseSingle player)
    {
        List<string> arrayAux = new List<string>();
        foreach(CardAttack card in player.table.CardNaMesa)
        {
            arrayAux.Add(card.ConditionBattle);
        }

        return arrayAux;
    }
    
    // Início das batalhas de cartas, configurado como batalha entre as últimas cartas jogadas
    private void Battle()
    {
        CardAttack card = null;
        CardAttack cardinimigo = null;

        if (isBattle)
        {   
            card = player1.table.CardNaMesa[player1.table.CardNaMesa.Count - 1];
            if (VerificarCardsNaMesa(cpu))
            {
                cardinimigo = cpu.table.CardNaMesa[cpu.table.CardNaMesa.Count - 1];
            }
            else
            {
                Reiniciar();
                return;
            }

            if (card.ConditionBattle == cardinimigo.ConditionBattle)
            {
                List<string> array = SelectBattleCards(player1);
                List<string> arrayCpu = SelectBattleCards(cpu);

                if (card.ConditionBattle == "attack")
                {
                    CardAttack cardAux;
                    CardAttack cardAuxCpu;
                    // variáveis auxiliares
                    var condicionBattleTemp = "defense";
                    int indiceCardTempCpu = arrayCpu.FindIndex(condicionBattleTemp.StartsWith);
                    int indiceCardTempPlayer = array.FindIndex(condicionBattleTemp.StartsWith);

                    if (indiceCardTempCpu == -1 || indiceCardTempPlayer == -1)
                    {
                        if(indiceCardTempCpu == -1)
                        {
                            cpu.CustoLife(System.Math.Abs(card.attack));
                        }
                        if (indiceCardTempPlayer == -1)
                        {
                            player1.CustoLife(System.Math.Abs(cardinimigo.attack));
                        }

                        Destroy(card.gameObject);
                        Destroy(cardinimigo.gameObject);

                        player1.table.CardNaMesa.Remove(card);
                        cpu.table.CardNaMesa.Remove(cardinimigo);

                        player1.table.ReorganizeTable();
                        cpu.table.ReorganizeTable();

                        player1.atualizarAtributos();
                        
                        
                    }
                    if(indiceCardTempPlayer != -1)
                    {
                        cardAux = player1.table.CardNaMesa[indiceCardTempPlayer];
                        AplicarDanoCardTargetPlayer(cardAux, cardinimigo);
                    }
                    if(indiceCardTempCpu != -1)
                    {
                        cardAuxCpu = cpu.table.CardNaMesa[indiceCardTempCpu];
                        AplicarDanoCardTargetCpu(cardAuxCpu, card);
                    }

                    VerificarTerminoTurno();
                    if (estilo == 4)
                    {
                        player1.CustoMana(-1);
                        auxEstilo = UnityEngine.Random.Range(1, 4);
                        Debug.Log("Alternando: " + auxEstilo);
                    }
                    return;

                } else if(card.ConditionBattle == "defense")
                {
                    int quantityLoop = (array.Count > arrayCpu.Count || array.Count == arrayCpu.Count) ?
                array.Count : arrayCpu.Count;
                    if (quantityLoop > 1)
                    {
                        int indiceattackP1 = 0;
                        int indicedefenseCpu = 0;
                        for (int i = 0; i < 2; i++)
                        {
                            if (!(i >= array.Count))
                            {
                                if (array[i] == "attack")
                                {
                                    card = player1.table.CardNaMesa[i];
                                    indiceattackP1++;
                                    Debug.Log("Card attack player 1 capturado");
                                }
                            }
                            if (!(i >= arrayCpu.Count))
                            {
                                if (arrayCpu[i] == "defense")
                                {
                                    cardinimigo = cpu.table.CardNaMesa[i];
                                    indicedefenseCpu++;
                                    Debug.Log("Card defesa cpu capturado");
                                }
                            }

                            if(!(indiceattackP1 > 0))
                            {
                                if(array[i] == "defense")
                                {
                                    card = player1.table.CardNaMesa[i];
                                    Debug.Log("Card defesa player 1 capturado");
                                }
                            }
                            if (!(indicedefenseCpu > 0))
                            {
                                if(arrayCpu[i] == "attack")
                                {
                                    cardinimigo = cpu.table.CardNaMesa[i];
                                    Debug.Log("Card attack cpu capturado");
                                }
                            }
                        }

                        if(card != null && cardinimigo != null)
                        {
                            if(card.ConditionBattle == "attack")
                            {
                                AplicarDanoCardTargetCpu(cardinimigo, card);
                            } else
                            {
                                AplicarDanoCardTargetPlayer(card, cardinimigo);
                                player1.defesaJogador.GetComponent<TextMeshProUGUI>().text =
                                    card.getCurrentResistance.ToString();
                            }
                        }
                    }
                }
            } else if (card.ConditionBattle == "attack")
            {
                AplicarDanoCardTargetCpu(cardinimigo, card);
            }
            else
            {
                AplicarDanoCardTargetPlayer(card, cardinimigo);
            }
            VerificarTerminoTurno();
            if (estilo == 4)
            {
                player1.CustoMana(-1);
                auxEstilo = UnityEngine.Random.Range(1, 4);
                Debug.Log("Alternando: " + auxEstilo);
            }
        }
    }

    // Atribuição de pontos para o turno que zerou as cartas da mão
    public bool AtribuirPontos()
    {
        if (player1.hand.Cards.Count <= 0 || cpu.hand.Cards.Count <= 0 && !isBattle)
        {
            if (cpu.getLife < player1.getLife)
                player1.pontos++;
            else if (cpu.getLife == player1.getLife)
            {
                cpu.pontos++;
                player1.pontos++;
            }
            else
            {
                player1.CustoLife(1);
                cpu.pontos++;
            }

            endTurn = true;
        } else if(player1.getLife <= 0)
        {
            vencedor = "Cpu Venceu!";

            player1.jogadaVez = true;
            TerminarJogo(player1);
            TerminarJogo(cpu);
            Debug.Log(vencedor);
            endTurn = true;
        } else if(cpu.getLife <= 0)
        {
            vencedor = "Player Venceu!";

            player1.jogadaVez = true;
            TerminarJogo(player1);
            TerminarJogo(cpu);
            Debug.Log(vencedor);
            endTurn = true;
        }
        return endTurn;
    }

    /* Início de métodos para aplique de dano ao jogador ou cpu. */
    private void AplicarDanoCardTargetCpu(CardAttack cardinimigo, CardAttack card)
    {
        int dano = 0;
        cardinimigo.CustoDefesa(card.attack);
        computer.defesaCpu.GetComponentInChildren<TextMeshProUGUI>().text =
            cardinimigo.getCurrentResistance.ToString();

        Debug.Log("Dano atribuído a Cpu: " + card.attack);
        if (cardinimigo.getCurrentResistance <= 0)
        {
            dano = cardinimigo.getCurrentResistance;
            Destroy(cardinimigo.gameObject);
            cpu.table.CardNaMesa.Remove(cardinimigo);
            cpu.table.ReorganizeTable();
            if (dano < 0)
            {
                cpu.CustoLife(System.Math.Abs(dano));
            }
            computer.defesaCpu.SetActive(false);
        }

    }

    private void AplicarDanoCardTargetPlayer(CardAttack card, CardAttack cardinimigo)
    {
        int dano = 0;
        card.CustoDefesa(cardinimigo.attack);
        player1.defesaJogador.GetComponentInChildren<TextMeshProUGUI>().text =
            card.getCurrentResistance.ToString();
        if (card.getCurrentResistance <= 0)
        {
            dano = card.getCurrentResistance;
            Destroy(card.gameObject);
            player1.table.CardNaMesa.Remove(card);
            player1.table.ReorganizeTable();

            player1.table.ReorganizeTable();
            if (dano < 0)
            {
                player1.CustoLife(System.Math.Abs(dano));
                player1.atualizarAtributos();
            }
            player1.defesaJogador.SetActive(false);
        }
    }
    /* Fim dos métodos de aplique de danos */

    // Início de um novo turno
    private void NovoTurno()
    {
        currentTurn++;
        // variáveis para distribuição das cartas do deck
        player1.SetInit = true;
        cpu.SetInit = true;

        // variáveis de controle da vez, turno e botões do player
        player1.jogadaVez = true;
        isBattle = false;
        endTurn = false;
        player1.ControleBotoes(false);

        player1.defesaJogador.SetActive(false);
        computer.defesaCpu.SetActive(false);

        // Limpando a mesa e zerando o contador das cartas na mão dos players
        LimparMesa(player1);
        LimparMesa(cpu);
        player1.ZerarCountCards();
        cpu.ZerarCountCards();
    }

    // Limpar carta da mão do player passado por parâmetro
    private void LimparCardsMao(PlayerBaseSingle player)
    {
        foreach(CardAttack card in player.hand.Cards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }
        player.hand.Cards.Clear();
    }

    /* Reinício da partida a partir do critério de player sem mana, 
     * entrega ponto ao adversário e passa para um novo turno
     * */
    public void ManaZeroReiniciar(PlayerBaseSingle player)
    {
        player.pontos++;
        LimparCardsMao(player1);
        LimparCardsMao(cpu);
        endTurn = true;
        NovoTurno();
    }

    // Reiniciar a partida do player1 e restaurando a mana de ambos
    private void Reiniciar()
    {
        ManaZeroReiniciar(player1);
        cpu.RestaurarMana();
        player1.RestaurarMana();
        player1.atualizarAtributos();
        player1.btnSemMana.GetComponent<Button>().interactable = false;

        player1.defesaJogador.SetActive(false);
        computer.defesaCpu.SetActive(false);
    }

    // Limpa as cartas da mesa do player passado por parâmetro
    private void LimparMesa(PlayerBaseSingle player)
    {
        foreach(CardBase card in player.table.CardNaMesa)
        {
            if(card != null)
                Destroy(card.gameObject);
        }
    }

    // Verifica se existe cartas na mesa do player passado por parâmetro
    private bool VerificarCardsNaMesa(PlayerBaseSingle player)
    {
        if(player.table.CardNaMesa.Count <= 0)
        {
            return false;
        }
        return true;
    }

    // Verifica se houve o término do turno
    private void VerificarTerminoTurno()
    {
        if (AtribuirPontos())
        {
            NovoTurno();
        }
        else
        {
            player1.jogadaVez = true;
            isBattle = false;
        }
    }


    // Finaliza a partida
    private void FimJogo()
    {
        bool endMatch = false;
        if(player1.getLife == 0)
        {
            vencedor = "Perdeu!!";
            endMatch = true;
        } else if(cpu.getLife == 0)
        {
            vencedor = "Player venceu!!";
            endMatch = true;
        }
        else if(currentTurn == 4)
        {
            if(player1.pontos > cpu.pontos)
            {
                vencedor = "Player venceu!!";
            }
            else if(player1.pontos < cpu.pontos)
            {
                vencedor = "Perdeu!!";
            }
            else if(player1.getLife < cpu.getLife)
            {
                vencedor = "Perdeu!!";
            }
            else
            {
                vencedor = "Player venceu!!";
            }
            endMatch = true;
        }

        /* Garante terminar o jogo somente se houver um vencedor,
         * limpando as cartas da mesa e apresentando o Canvas de fim de jogo
         * */
        if (endMatch)
        {
            TextMeshProUGUI[] textosPanel = panelVencedor.GetComponentsInChildren<TextMeshProUGUI>();

            player1.ControleBotoes(false);
            player1.jogadaVez = true;
            player1.defesaJogador.SetActive(false);
            TerminarJogo(player1);
            TerminarJogo(cpu);

            DistribuirPontuacao(faseAtual);

            panelVencedor.SetActive(true);
            textosPanel[0].SetText(vencedor);
            textosPanel[2].SetText(player1.GetPontuacao.ToString());
            if (!controlRank && player1.GetPontuacao != 0)
            {
                RankingController.CriarArquivo(player1.GetPontuacao);
                controlRank = true;
                // registrando a fase cumprida
                ControllerFases.CriarArquivo(faseAtual);
            }
        }
    }

    // Método de finalização de jogo, por questão de reaproveitamento
    private void TerminarJogo(PlayerBaseSingle player)
    {
        LimparCardsMao(player);
        LimparMesa(player);
    }

    /*
     *  Método que realiza a distribuição da pontuação de acordo com a
     *  fase passada por parâmetro.
     * */
    public void DistribuirPontuacao(int fase)
    {
        int[] pontos = Pontuacao(fase);

        if (player1.GetPontuacao == 0 && pontos != null)
        {
            if(cpu.getLife == 0)
            {
                player1.GetPontuacao = pontos[0];
            }
            else if (player1.pontos == 3)
            {
                player1.GetPontuacao = pontos[1];
            }
            else if (player1.pontos == 3 && player1.getLife == player1.totalLife)
            {
                player1.GetPontuacao = 100;
            }
            else if (player1.pontos == 2 && cpu.pontos == 1)
            {
                player1.GetPontuacao = pontos[2];
            }
            else
            {
                player1.GetPontuacao = 0;
            }
        }
    }

    /*
     *  Definição da quantidade de pontos de acordo com a fase atribuído em
     *  um array e o retornando
     * */
    private int[] Pontuacao(int fase)
    {
        int[] pontos = null;
        switch (fase)
        {
            case 1:
                pontos = new int[] { 15, 20, 10 };
                break;

            case 2:
                pontos = new int[] { 16, 21, 5 };
                break;

            case 3:
                pontos = new int[] { 13, 25, 14 };
                break;

            case 4:
                pontos = new int[] { 10, 30, 16 };
                break;

            case 5:
                pontos = new int[] { 10, 35, 17 };
                break;

            case 6:
                pontos = new int[] { 10, 40, 20 };
                break;

            case 7:
                pontos = new int[] { 18, 55, 28 };
                break;

            case 8:
                pontos = new int[] { 20, 60, 30 };
                break;

            case 9:
                pontos = new int[] { 25, 75, 40 };
                break;

            case 10:
                pontos = new int[] { 30, 95, 90 };
                break;
        }
        return pontos;
    }
}
