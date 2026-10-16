# ONSHAPE FEATURESCRIPT — PARAMETRIC KNURLING

[English](README.md) · [中文](README.zh-CN.md) · [Français](README.fr.md) · [Italiano](README.it.md) · [Türkçe](README.tr.md)

Outils de moletage paramétrique pour le design industriel, développés avec Onshape FeatureScript et une aide de l’IA à la programmation.

Ce projet explore comment un designer peut transformer un besoin concret de détail de surface en outil CAO réutilisable. Il privilégie une géométrie modifiable et des variations contrôlées pour comparer la prise en main, les proportions visuelles, l’intention tactile et les détails de surface pendant le développement produit.

## WHY ONSHAPE

Un moletage dense génère de nombreuses faces, arêtes et intersections répétées. Lorsque le motif se complexifie, la régénération peut devenir une opération exigeante sur un poste de travail local.

L’architecture cloud-native d’Onshape est particulièrement intéressante ici : la géométrie est régénérée dans le cloud, tandis que le designer travaille dans un navigateur. Cela réduit la dépendance à un seul poste pour les calculs de modélisation et permet de travailler depuis différents appareils. Les motifs complexes nécessitent toujours des calculs ; le temps de régénération dépend de la géométrie et de l’implémentation de la fonction.

FeatureScript transforme la logique du motif en fonction personnalisée réutilisable, dont les réglages restent intégrés au processus habituel de modélisation Onshape.

## AI-ASSISTED DEVELOPMENT

Le moletage est une application établie qui peut déjà être modélisée de façon paramétrique. Cette étude de cas explore comment les connaissances existantes en modélisation paramétrique peuvent guider la programmation assistée par IA et devenir une composante réutilisable du processus de conception.

Les exemples paramétriques existants servent de référence : leur géométrie, leurs paramètres et leur logique de modélisation aident à expliquer à l’IA le comportement attendu. Par des instructions itératives et des essais dans Onshape, cette logique est traduite en FeatureScript.

L’objectif est d’intégrer plus efficacement les méthodes de modélisation établies au processus de conception. Une fonction personnalisée réutilisable permet au designer de régler les paramètres et de comparer les variantes tout en maîtrisant l’intention de conception et en évaluant la géométrie obtenue.

## DESIGN WORKFLOW

1. Ajouter le FeatureScript personnalisé à Onshape.
2. Ouvrir la fonction depuis la barre d’outils.
3. Régler les paramètres de moletage.
4. Régénérer et comparer les variantes.

## FEATURESCRIPT VARIANTS

Chaque fichier crée une nouvelle pièce indépendante suivant l’axe Z. Utiliser un Feature Studio distinct pour chaque variante ; les scripts partagent des noms d’énumérations et de fonctions auxiliaires.

| FILE | GEOMETRY AND COVERAGE |
| --- | --- |
| [Cylindre — Surface complète](featurescripts/knurl-cylinder-full-surface.fs) | Cylindre sans marges lisses ni bandes axiales. |
| [Cylindre — En retrait](featurescripts/knurl-cylinder-inset.fs) | Cylindre avec marges supérieure et inférieure et bandes axiales facultatives. |
| [Prisme — Surface complète](featurescripts/knurl-prism-full-surface.fs) | Prisme carré ou rectangulaire avec motif sur les quatre faces latérales. |
| [Prisme — En retrait](featurescripts/knurl-prism-inset.fs) | Prisme carré ou rectangulaire avec bordures supérieure, inférieure et latérales. |

« Inset » désigne un motif en retrait des bords, et non un moletage d’alésage intérieur.

## PARAMETERS

Les valeurs suivantes proviennent des déclarations de paramètres FeatureScript 2716 fournies. Elles sont présentées sous la forme **valeur par défaut (plage déclarée)** ; les contrôles géométriques peuvent limiter davantage les combinaisons valides.

| CONTROL | DEFAULT (DECLARED RANGE) |
| --- | --- |
| Diamètre du cylindre / largeur du prisme | 30 mm (2–500 mm) |
| Profondeur du prisme rectangulaire | 20 mm (2–500 mm) ; section carrée activée par défaut |
| Longueur du corps / hauteur du prisme | 40 mm (2–1000 mm) |
| Nombre de rainures du cylindre, par direction | 32 (8–120) |
| Nombre de rainures sur le périmètre développé du prisme | 48 (8–240), arrondi à un multiple de quatre |
| Pas demandé, en alternative au nombre | 2.5 mm (0.2–50 mm) |
| Profondeur des rainures | 0.45 mm (0.02–10 mm) |
| Angle d’hélice / diagonal par rapport à l’axe du corps | 30° (5–65°) |
| Angle inclus du profil en V | 120° (30–150°) |
| Rapport de largeur du méplat de crête, en alternative à l’angle en V | 0.2 (0.05–0.85) |
| Décalage du chanfrein d’extrémité / rayon du congé | 0.8 mm (0.01–100 mm) |
| Angle de chanfrein | 45° (10–80°) |
| Rayon de congé des coins du prisme / décalage du chanfrein | 0.8 mm (0.01–100 mm) |
| Marges supérieure / inférieure du cylindre en retrait | 2 mm (0–500 mm) |
| Bordures supérieure / inférieure / latérales du prisme en retrait | 2 mm (0–200 mm) |
| Bandes axiales du cylindre en retrait | 1 (1–6) ; intervalle 3 mm (0.1–100 mm) |
| Phase du motif | 0° (0–360°) |
| Largeur du méplat en fond de rainure | 0 mm (0–5 mm) |
| Rayon facultatif d’arrondi des crêtes / fonds | 0.05 mm (0.005–2 mm) |
| Dépassement des outils, variantes à surface complète et prisme en retrait | 1 mm (0–50 mm) à chaque extrémité |

Toutes les variantes proposent des motifs diamant, droits et sans motif, ainsi que des motifs hélicoïdaux pour les cylindres ou diagonaux pour les prismes. Les modes de moletage sont en creux, en relief ou moitié en relief / moitié en creux. Les références dimensionnelles distinguent les dimensions nominales du corps des dimensions extérieures maximales. Les extrémités peuvent être vives, chanfreinées ou arrondies.

Le pas est une valeur demandée : les scripts calculent un nombre entier de répétitions, le pas réel peut donc différer. L’affichage des commandes dépend du style, de la densité, du profil et des réglages avancés.

## IMPLEMENTATION STATUS

Le code source fourni est conservé sans modification, y compris les identifiants des fonctions et la version 2716 de la bibliothèque. Les vérifications du dépôt confirment l’organisation des fichiers et la documentation des paramètres ; aucune compilation ni régénération dans Onshape n’a été effectuée dans le cadre de cette mise à jour.

## DESIGN EVALUATION

Évaluer chaque variante selon la prise en main, le caractère tactile, la qualité des arêtes et la cohérence avec la forme et le matériau du produit. Évaluer l’aptitude à la fabrication selon le procédé et les dimensions prévus.
