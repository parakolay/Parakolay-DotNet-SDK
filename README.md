
# Parakolay .NET SDK

Bu SDK, .NET Core ortamında Parakolay API kullanımını kolaylaştırmak için geliştirilmiş bir yazılımdır. SDK, [Parakolay](https://www.parakolay.com) ödeme sistemi ile etkileşim kurmak için gerekli tüm işlevselliği sağlar. Bu dokümantasyon, SDK'nın nasıl kurulacağını, yapılandırılacağını ve kullanılacağını adım adım gösterir.

## Kurulum

SDK'yı kullanabilmek için öncelikle .NET Core projenize SDK projesini Dependency olarak eklemeniz gerekmektedir. Bu SDK, .NET Core 3.1 veya daha yeni sürümlerle uyumludur.

## API Anahtarları

SDK'yı kullanabilmek için Parakolay tarafından sağlanan API anahtarlarına ihtiyacınız vardır. Bu anahtarlar şunlardır:

- `apiKey`
- `apiSecret`
- `merchantNumber`

Bu bilgileri [Parakolay](https://www.parakolay.com) hesabınızdan edinebilirsiniz.

## Başlangıç

SDK'yı projenize ekledikten sonra, aşağıdaki gibi kullanarak ödeme işlemlerini başlatabilirsiniz:

```csharp
        string apiKey = "YOUR_API_KEY";
        string apiSecret = "YOUR_API_SECRET";
        string merchantNumber = "YOUR_MERCHANT_NUMBER";
        string conversationId = "YOUR_ORDER_ID";
        string clientIpAddress = "CLIENT_IP_ADDRESS";

        Parakolay apiClient = new Parakolay("API_BASE_URL", apiKey, apiSecret, merchantNumber, "CONVERSATION_ID", "CLIENT_IP");
        // API fonksiyonları
    }
}
```

## Ödeme İşlemleri

### 3D Secure Başlatma

3D Secure ödeme işlemi başlatmak için `Init3DS` metodunu kullanabilirsiniz. Örnek kullanım aşağıdaki gibidir:

```csharp
Init3dsResponseModel result = await apiClient.Init3DS("KART_NUMARASI", "KART_SAHIBI_ADI", "SKT_AY", "SKT_YIL", "CVV", miktar, puanMiktarı, "CALLBACK_URL");
```

### 3D Secure Tamamlama

Kullanıcı 3D Secure doğrulamasını tamamladıktan sonra, ödeme işlemini tamamlamak için `Complete3DS` metodunu kullanabilirsiniz:

```csharp
var completeResult = await apiClient.Complete3DS(result);
```

## Hata Yönetimi

SDK, API çağrıları sırasında oluşabilecek hataları yönetmek için kapsamlı bir yapı sunar. Her API metodu, başarılı bir sonuç veya hata detayları içeren bir yanıt döndürür.

# Destek ve Katkıda Bulunma

Bu kütüphane ile ilgili sorunlarınız veya önerileriniz varsa, lütfen GitHub üzerinden bir issue açın. Ayrıca, kütüphaneye katkıda bulunmak istiyorsanız, pull request'lerinizi bekliyoruz.

## Lisans

Bu proje [MIT Lisansı](LICENSE) altında lisanslanmıştır.