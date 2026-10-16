# Godronatura parametrica per Onshape

[English](README.md) · [中文](README.zh-CN.md) · [Français](README.fr.md) · [Italiano](README.it.md) · [Türkçe](README.tr.md)

Quattro strumenti FeatureScript per creare una godronatura modificabile su parti cilindriche, quadrate e rettangolari, sviluppati per il design industriale con il supporto dell’IA alla programmazione.

[Umut Yurdagül](https://github.com/umutyurdagul-afk) · Design industriale

## Demo

**[Guarda il video](https://github.com/umutyurdagul-afk/onshape-parametric-knurling/blob/main/media/onshape-parametric-knurling-demo.mp4)** · [Scarica MP4](https://raw.githubusercontent.com/umutyurdagul-afk/onshape-parametric-knurling/main/media/onshape-parametric-knurling-demo.mp4)

Una dimostrazione del progetto in Onshape.

## Varianti degli strumenti

Ogni file crea una nuova parte indipendente lungo l’asse Z. Usare un Feature Studio separato per ogni variante; gli script condividono nomi di enumerazioni e funzioni ausiliarie.

| File | Geometria e copertura |
| --- | --- |
| [Cilindro — Superficie completa](featurescripts/knurl-cylinder-full-surface.fs) | Cilindro senza margini non zigrinati né fasce assiali. |
| [Cilindro — Con margini](featurescripts/knurl-cylinder-inset.fs) | Cilindro con margini superiore e inferiore e fasce assiali opzionali. |
| [Prisma — Superficie completa](featurescripts/knurl-prism-full-surface.fs) | Prisma quadrato o rettangolare con motivo sulle quattro facce laterali. |
| [Prisma — Con margini](featurescripts/knurl-prism-inset.fs) | Prisma quadrato o rettangolare con bordi superiore, inferiore e laterali. |

« Inset » indica un motivo arretrato rispetto ai bordi, non la zigrinatura di un foro interno.

## Flusso di progettazione

1. Aggiungere il FeatureScript personalizzato a Onshape.
2. Aprire la funzione dalla barra degli strumenti.
3. Regolare i parametri di zigrinatura.
4. Rigenerare e confrontare le varianti.

## Parametri

I valori seguenti provengono dalle dichiarazioni dei parametri FeatureScript 2716 caricati. Sono indicati come **valore predefinito (intervallo dichiarato)**; i controlli geometrici possono limitare ulteriormente le combinazioni valide.

| Parametro | Valore predefinito (intervallo dichiarato) |
| --- | --- |
| Diametro del cilindro / larghezza del prisma | 30 mm (2–500 mm) |
| Profondità del prisma rettangolare | 20 mm (2–500 mm); sezione quadrata attiva per impostazione predefinita |
| Lunghezza del corpo / altezza del prisma | 40 mm (2–1000 mm) |
| Numero di scanalature del cilindro, per direzione | 32 (8–120) |
| Numero di scanalature sul perimetro sviluppato del prisma | 48 (8–240), arrotondato a un multiplo di quattro |
| Passo richiesto, alternativo al numero | 2.5 mm (0.2–50 mm) |
| Profondità delle scanalature | 0.45 mm (0.02–10 mm) |
| Angolo elicoidale / diagonale rispetto all’asse del corpo | 30° (5–65°) |
| Angolo incluso del profilo a V | 120° (30–150°) |
| Rapporto del tratto piano di cresta, alternativo all’angolo a V | 0.2 (0.05–0.85) |
| Distanza dello smusso di estremità / raggio del raccordo | 0.8 mm (0.01–100 mm) |
| Angolo dello smusso | 45° (10–80°) |
| Raggio di raccordo degli angoli del prisma / distanza dello smusso | 0.8 mm (0.01–100 mm) |
| Margini superiore / inferiore del cilindro inset | 2 mm (0–500 mm) |
| Bordi superiore / inferiore / laterali del prisma inset | 2 mm (0–200 mm) |
| Fasce assiali del cilindro inset | 1 (1–6); distanza 3 mm (0.1–100 mm) |
| Fase del motivo | 0° (0–360°) |
| Larghezza del tratto piano al fondo della scanalatura | 0 mm (0–5 mm) |
| Raggio opzionale di arrotondamento di creste / fondi | 0.05 mm (0.005–2 mm) |
| Sovracorsa degli utensili, varianti a superficie completa e prisma inset | 1 mm (0–50 mm) a ogni estremità |

Tutte le varianti offrono motivi a diamante, diritti e senza motivo, oltre a motivi elicoidali per i cilindri o diagonali per i prismi. Le modalità di zigrinatura sono a incavo, in rilievo o metà in rilievo / metà a incavo. I riferimenti dimensionali distinguono le dimensioni nominali del corpo dalle dimensioni esterne massime. Le estremità possono essere a spigolo vivo, smussate o raccordate.

Il passo è un valore richiesto: gli script ricavano un numero intero di ripetizioni, quindi il passo effettivo può differire. I controlli sono visibili in base a stile, densità, profilo e impostazioni avanzate.

## Perché Onshape

Una zigrinatura fitta genera facce, spigoli e intersezioni ripetuti. Con l’aumentare della complessità del motivo, la rigenerazione può diventare un’operazione impegnativa su una workstation locale.

L’approccio cloud-native di Onshape è particolarmente utile: la rigenerazione della geometria avviene nel cloud, mentre il designer lavora nel browser. Questo riduce la dipendenza da una singola workstation per i calcoli di modellazione e rende il lavoro accessibile da dispositivi diversi. I motivi complessi richiedono comunque calcolo; il tempo di rigenerazione dipende dalla geometria e dall’implementazione della funzione.

FeatureScript trasforma la logica del motivo in una funzione personalizzata riutilizzabile, mantenendo le regolazioni nel normale flusso di modellazione di Onshape.

## Sviluppo assistito dall’IA

Il progetto esplora come un designer possa trasformare un’esigenza concreta di dettaglio superficiale in uno strumento CAD riutilizzabile. L’attenzione è rivolta alla geometria modificabile e alle variazioni controllate: confrontare presa, proporzioni visive, intento tattile e dettagli superficiali durante lo sviluppo del prodotto.

La zigrinatura è un’applicazione consolidata che può già essere modellata parametricamente. Questo caso di studio esplora come le conoscenze esistenti di modellazione parametrica possano guidare la programmazione assistita dall’IA e diventare una parte riutilizzabile del flusso di progettazione.

Gli esempi parametrici esistenti costituiscono il riferimento: geometria, parametri e logica di modellazione aiutano a spiegare all’IA il comportamento desiderato. Attraverso indicazioni iterative e prove in Onshape, questa logica viene tradotta in FeatureScript.

L’obiettivo è integrare più efficacemente metodi di modellazione consolidati nel flusso di progettazione. Una funzione personalizzata riutilizzabile permette al designer di regolare i parametri e confrontare le varianti, mantenendo il controllo sull’intento progettuale e valutando la geometria risultante.

## Stato dell’implementazione

Il sorgente caricato è conservato senza modifiche, inclusi gli identificatori delle funzioni e la versione 2716 della libreria. I controlli del repository confermano l’organizzazione dei file e la documentazione dei parametri; nell’ambito di questo aggiornamento non sono state eseguite compilazioni o rigenerazioni in Onshape.

## Valutazione del design

Valutare ogni variante per presa, carattere tattile, qualità degli spigoli e coerenza con forma e materiale del prodotto. Verificare l’idoneità alla produzione rispetto al processo e alle dimensioni previsti.
