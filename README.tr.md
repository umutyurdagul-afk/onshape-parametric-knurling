# Onshape için Parametrik Tırtıl

[English](README.md) · [中文](README.zh-CN.md) · [Français](README.fr.md) · [Italiano](README.it.md) · [Türkçe](README.tr.md)

Silindirik, kare ve dikdörtgen parçalar için düzenlenebilir tırtıl geometrisi üreten dört FeatureScript aracı. Endüstriyel tasarım için AI destekli kodlama ile geliştirildi.

[Umut Yurdagül](https://github.com/umutyurdagul-afk) · Endüstriyel Tasarım

## Demo

**[Tanıtım videosunu izle](https://github.com/umutyurdagul-afk/onshape-parametric-knurling/blob/main/media/onshape-parametric-knurling-demo.mp4)** · [MP4 indir](https://raw.githubusercontent.com/umutyurdagul-afk/onshape-parametric-knurling/main/media/onshape-parametric-knurling-demo.mp4)

Projenin Onshape içindeki kullanımını gösteren video.

## Araç sürümleri

Her dosya Z ekseni boyunca yeni, bağımsız bir parça oluşturur. Her sürüm için ayrı bir Feature Studio kullan; betikler aynı enum ve yardımcı işlev adlarını paylaşır.

| Dosya | Geometri ve kapsam |
| --- | --- |
| [Silindir — Tam Yüzey](featurescripts/knurl-cylinder-full-surface.fs) | Desensiz kenar boşluğu veya eksenel bant içermeyen silindir. |
| [Silindir — Inset](featurescripts/knurl-cylinder-inset.fs) | Üst/alt kenar boşlukları ve isteğe bağlı eksenel bantlar içeren silindir. |
| [Prizma — Tam Yüzey](featurescripts/knurl-prism-full-surface.fs) | Dört yan yüzeyi desenli kare veya dikdörtgen prizma. |
| [Prizma — Inset](featurescripts/knurl-prism-inset.fs) | Üst, alt ve yan kenar boşlukları içeren kare veya dikdörtgen prizma. |

“Inset”, iç deliğin tırtıllanmasını değil, desenin kenarlardan içeride kalmasını ifade eder.

## Tasarım iş akışı

1. Özel FeatureScript’i Onshape’a ekle.
2. Özelliği araç çubuğundan aç.
3. Tırtıl parametrelerini ayarla.
4. Modeli yeniden hesapla ve varyasyonları karşılaştır.

## Parametreler

Aşağıdaki değerler yüklenen FeatureScript 2716 parametre tanımlarından alınmıştır. Değerler **varsayılan (tanımlanan aralık)** şeklindedir; geometri kontrolleri geçerli kombinasyonları ayrıca sınırlayabilir.

| Parametre | Varsayılan (tanımlı aralık) |
| --- | --- |
| Silindir çapı / prizma genişliği | 30 mm (2–500 mm) |
| Dikdörtgen prizma derinliği | 20 mm (2–500 mm); varsayılan olarak kare kesit |
| Gövde uzunluğu / prizma yüksekliği | 40 mm (2–1000 mm) |
| Silindir oluk sayısı, her yön için | 32 (8–120) |
| Prizmanın açınım çevresi boyunca oluk sayısı | 48 (8–240), dördün katına yuvarlanır |
| Sayıya alternatif olarak istenen adım | 2.5 mm (0.2–50 mm) |
| Oluk derinliği | 0.45 mm (0.02–10 mm) |
| Gövde ekseninden helis / çapraz açı | 30° (5–65°) |
| V profilinin iç açısı | 120° (30–150°) |
| V açısına alternatif tepe düzlüğü oranı | 0.2 (0.05–0.85) |
| Uç pah mesafesi / radyüs yarıçapı | 0.8 mm (0.01–100 mm) |
| Pah açısı | 45° (10–80°) |
| Prizma köşe radyüsü / pah mesafesi | 0.8 mm (0.01–100 mm) |
| Inset silindirin üst / alt boşlukları | 2 mm (0–500 mm) |
| Inset prizmanın üst / alt / yan boşlukları | 2 mm (0–200 mm) |
| Inset silindirin eksenel bantları | 1 (1–6); bant aralığı 3 mm (0.1–100 mm) |
| Desen fazı | 0° (0–360°) |
| Oluk kökü düzlüğünün genişliği | 0 mm (0–5 mm) |
| İsteğe bağlı tepe / kök yuvarlatma yarıçapı | 0.05 mm (0.005–2 mm) |
| Kesici taşması, tam yüzey sürümleri ve inset prizma | Her uçta 1 mm (0–50 mm) |

Tüm sürümlerde baklava, düz ve desensiz seçenekler vardır; silindirlerde helisel, prizmalarda çapraz desen de bulunur. Tırtıl modları kesme, kabartma veya yarı kabartma / yarı kesmedir. Boyut referansları, nominal gövde ölçüleri ile maksimum dış ölçüleri ayırır. Uçlar keskin, pahlı veya radyüslü olabilir.

Adım, istenen değerdir: betikler tam sayılı tekrar sayısı hesapladığı için gerçek adım farklı olabilir. Kontroller; desen, yoğunluk, profil ve gelişmiş ayarlara bağlı olarak görünür.

## Neden Onshape?

Yoğun tırtıl desenleri tekrarlanan yüzeyler, kenarlar ve kesişimler oluşturur. Desen karmaşıklaştıkça modeli yeniden hesaplamak yerel bir iş istasyonunda ağır bir işleme dönüşebilir.

Onshape’ın bulut tabanlı mimarisi burada değerlidir: geometri bulutta yeniden hesaplanırken tasarımcı tarayıcı üzerinden çalışır. Bu yaklaşım, modelleme hesaplarında tek bir iş istasyonuna bağımlılığı azaltır ve farklı cihazlardan çalışmayı mümkün kılar. Karmaşık desenler yine hesaplama gerektirir; süre geometriye ve özelliğin uygulanışına bağlıdır.

FeatureScript, desen mantığını yeniden kullanılabilir bir özel özelliğe dönüştürür; ayarlamalar normal Onshape modelleme sürecinde kalır.

## AI destekli geliştirme

Bu proje, tasarımcının pratik bir yüzey detaylandırma ihtiyacını yeniden kullanılabilir bir CAD aracına dönüştürmesini araştırır. Odak noktası düzenlenebilir geometri ve kontrollü varyasyonlardır: ürün geliştirme sırasında tutuş karakterini, görsel oranları, dokunsal amacı ve yüzey detaylarını karşılaştırmak.

Tırtıl oluşturma, hâlihazırda parametrik olarak modellenebilen yerleşik bir uygulamadır. Bu vaka çalışması, mevcut parametrik modelleme bilgisinin AI destekli kodlamayı nasıl yönlendirebileceğini ve tasarım sürecinin yeniden kullanılabilir bir parçasına nasıl dönüşebileceğini araştırır.

Mevcut parametrik örnekler referans sağlar: geometri, parametreler ve modelleme mantığı, beklenen davranışı AI’a anlatmaya yardımcı olur. Yinelemeli yönlendirme ve Onshape içindeki denemelerle bu mantık FeatureScript’e dönüştürülür.

Amaç, yerleşik modelleme yöntemlerini tasarım sürecine daha verimli dahil etmektir. Yeniden kullanılabilir bir özel özellik, tasarımcının tasarım niyetini kontrol ederek ve oluşan geometriyi değerlendirerek parametreleri ayarlamasını ve alternatifleri karşılaştırmasını sağlar.

## Uygulama durumu

Yüklenen kaynak kod, özellik tanımlayıcıları ve 2716 kütüphane sürümü dahil değiştirilmeden korunmuştur. Repo kontrolleri dosya düzenini ve parametre belgelerini doğrular; bu repo güncellemesi kapsamında Onshape’ta derleme veya yeniden hesaplama yapılmamıştır.

## Tasarım değerlendirmesi

Her varyasyonu tutuş, dokunsal karakter, kenar kalitesi ve ürünün biçimiyle ve malzemesiyle uyumu açısından değerlendir. Üretime uygunluğu hedeflenen işlem ve ölçülere göre incele.
