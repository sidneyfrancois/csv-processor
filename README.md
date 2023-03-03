![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![.Vscode](https://img.shields.io/badge/Made%20with-VSCode-1f425f.svg)
<img src="https://play-lh.googleusercontent.com/m0wKmUdoSnpwnhZpbin1gL7kzACIlq_s8QnqSS2RfR34GHw58OW1E1tbQ9RY7xgPqFA" width="140" 
height="140" align="right">

<br clear="left"/>

# Projeto de teste técnico (Auvo Tecnologia) :computer:



Este projet teve como prioridade:

- Organização de código
- Busca de maior performance
- Demonstrar domínio intermediário na linguagem (C#)
- Organização de commits (seguindo o padrão "conventional commits")

O que foi finalizado até o momento:
- [x] Processamento e mapeamento dos arquivos CSV
- [x] Geração do arquivo JSON com base nos dados do arquivo CSV
- [x] Aplicação de paralelismo
- [x] Uso de assincronismo (evitar I/O blocking)
- [ ] Criação de testes unitários
- [ ] Criação de componente plugavel

# Regras de Negócio

- Departamento
    - Total a Pagar: *Número total de horas * valor hora do funcionário.*
    - Total Descontos: *Número de horas debito * valor hora do funcionário.*
    - Total Extras: *Número de horas extras * valor hora do funcionário.*
- Funcionário
    - Total a Receber: *Total de horas trabalhadas * valor hora do funcionário*.
    - Horas Extras: O funcionário entrando antes ou depois do horário de trabalho oficial já é caracterizado como hora extra: Exemplos: funcionário deve entrar 8:00 mas entrou 6:00, serão contabilizadas 2 horas extras, funcinário saí oficialmente às 18:00 mas fica até 19:00, serão contabilizadas 1 hora extra.
    - Horas Débito: O funcionário entrando depois ou antes do horário de trabalho oficial já é caracterizado como hora débito: Exemplos: funcionário deve entrar 8:00 mas entrou 9:00, serão contabilizadas 1 hora débito, funcinário saí oficialmente às 18:00 mas saiu às 16:00, serão contabilizadas 2 horas débito.
    - Dias Falta: Se o funcionário faltar em qualquer dia da semana já será contabilizado como um dia de falta. Cada dia de falta corresponde a 8 horas de débito. (considerando que o funcionário tenha que trabalhar 8 horas por dia)
    - Dias Extra: Será contabilizado a partir do número total de horas extras, se o número de horas extras ultrapassar 24h será contabilizado como 1 dia extra. 
    - Dias Trabalhados: Será contabilizado a partir do número total de horas trabalhadas, se o número de horas trabalhadas ultrapassar 24h será contabilizado como 1 dia de trabalho. Exemplo: funcionário tem o total de 80 horas trabalhadas, isso será contabilizado como 3 dias trabalhados (mais 8 horas).
    
    *Detalhe: O funcionário trabalhando durante o fim de semana, durante qualquer horário, já terá as suas horas trabalhadas contabilizadas como hora extra. Exemplo: se o funcionário trabalhou no sábado das 8:00 até 12:00, serão contabilizadas 4 horas extras.*

# Uso de Paralelismo e Assincronismo

- Paralelismo: Para este projeto foi utilizado o TPL (Task Parallel Library) nativo do C#, recurso que permite utilizar o laço *ForEachAsync* para executar instruções dentro do laço de forma paralela e suportando metódo assíncrono.

- Assincronismo: A função *CsvMapAndProcessing* da classe *CSVProcessing* faz uso do de Streaming, para leitura dos arquivos csv, isso evita o uso excessivo de memória pois evita que os dados do arquivo csv sejam salvos primeiro em alguma variável local antes de serem processadas. A leitura dos arquivos é feita de maneira assíncrona através da função *ReadAsync*, evitando gargalo e aumentando a velocidade de processamento.

*Detalhe: Para este projeto foi utilizado a biblioteca CSVHelper. Algumas funcionalidades como o ReadAsync não estão propriamente documentadas e por isso foi necessário verificar o código fonte do projeto.*

# Utilização

1. fazer o download do código fonte deste projeto:
    ```
    git clone https://github.com/sidneyfrancois/csv-processor.git
    ```
2. Entrar dentro da pasta do projeto.
    ```
    cd csv-processor
    ```
3. Executar o projeto:
    ```
    dotnet run
    ```
4. O primeiro input é necessário para definir o caminho completo da pasta aonde os arquivos csv se encontram. Exemplo:
    ```
    Enter input Path
    C:\Users\nome_do_usuario\Documents\Auvo\csv-processor\PastaAondeEstaoOsArquivosCSV
    ```
5. O segundo input é necessário para definir o caminho completo da pasta aonde os arquivos json processados serão salvos. Exemplo:
    ```
    Enter Output Path
    C:\Users\nome_do_usuario\Documents\Auvo\csv-processor\PastaParaSalvarJSONs
    ```
6. O processamento será realizado e todos os arquivos json serão salvos na pasta específicada.
7. Será exibido o total de templo (em milissegundos) gasto no processamento e o total de memória utilizada.
    ```
    Time: 573 ms
    Memory Used: 36 mb
    ```
# Melhorias

- Diminuir o número de objetos e listas alocados durante o processamento.
- Diminuir redundancia na chamada das funções durante a coleta de dados para processar o json.
- Inserção de testes específicos para performance.
- unir todos os objetos json num só, json como todos os departamentos e seus respectivos funcionários.