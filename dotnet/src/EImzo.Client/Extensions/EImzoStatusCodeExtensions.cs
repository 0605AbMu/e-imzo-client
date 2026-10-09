using EImzo.Client.Enums;

namespace EImzo.Client.Extensions;

/// <summary>
/// Extension methods providing multi-language descriptions for E-IMZO status codes.
/// </summary>
public static class EImzoStatusCodeExtensions
{
    /// <summary>
    /// Gets a human-readable description for an <see cref="EImzoStatusCode"/> in the requested language (uz, ru, or en).
    /// </summary>
    /// <param name="code">The status code.</param>
    /// <param name="language">Language code ("uz", "ru", or "en"). Default is "en".</param>
    public static string GetDescription(this EImzoStatusCode code, string language = "en")
    {
        var lang = (language ?? "en").ToLowerInvariant();
        return code switch
        {
            EImzoStatusCode.Success => lang switch
            {
                "uz" => "Muvaffaqiyatli",
                "ru" => "Успешно",
                _ => "Success"
            },
            EImzoStatusCode.VpnOrCertificateCheckFailed => lang switch
            {
                "uz" => "Sertifikat holatini tekshirib bo'lmadi (VPN aloqasi mavjud emas yoki uzilgan)",
                "ru" => "Не удалось проверить статус сертификата. Часто это отсутствие VPN",
                _ => "Failed to verify certificate status (VPN connection might be down or unavailable)"
            },
            EImzoStatusCode.TimestampWindowExceeded => lang switch
            {
                "uz" => "Imzolash vaqti ruxsat etilgan oraliqdan tashqarida (kompyuter vaqtini tekshiring)",
                "ru" => "Время подписи вне допустимого окна (проверьте дату и время компьютера)",
                _ => "Signing time is outside the allowable window"
            },
            EImzoStatusCode.SignatureInvalid => lang switch
            {
                "uz" => "Elektron raqamli imzo (ERI / PKCS#7) haqiqiy emas",
                "ru" => "ЭЦП недействительна",
                _ => "Digital signature is invalid"
            },
            EImzoStatusCode.CertificateInvalid => lang switch
            {
                "uz" => "Sertifikat haqiqiy emas yoki bekor qilingan",
                "ru" => "Сертификат недействителен",
                _ => "Certificate is invalid or revoked"
            },
            EImzoStatusCode.CertificateInvalidAtSigningTime => lang switch
            {
                "uz" => "Sertifikat imzolangan vaqtda haqiqiy bo'lmagan",
                "ru" => "Сертификат недействителен на дату подписи",
                _ => "Certificate was not valid at the date and time of signing"
            },
            EImzoStatusCode.ChallengeNotFoundOrExpired => lang switch
            {
                "uz" => "Challenge topilmadi yoki uning amal qilish muddati tugagan",
                "ru" => "Не найден challenge или срок его истек",
                _ => "Challenge not found or has expired"
            },
            EImzoStatusCode.TimestampSignatureOrHashInvalid => lang switch
            {
                "uz" => "Vaqt tamg'asi (Timestamp) imzosi yoki heshi haqiqiy emas",
                "ru" => "ЭЦП или хеш Timestamp недействительны",
                _ => "Timestamp signature or hash is invalid"
            },
            EImzoStatusCode.TimestampCertificateInvalid => lang switch
            {
                "uz" => "Vaqt tamg'asi (Timestamp) sertifikati haqiqiy emas",
                "ru" => "Сертификат Timestamp недействителен",
                _ => "Timestamp certificate is invalid"
            },
            EImzoStatusCode.TimestampCertificateInvalidAtSigningTime => lang switch
            {
                "uz" => "Vaqt tamg'asi sertifikati imzolangan sanada haqiqiy bo'lmagan",
                "ru" => "Сертификат Timestamp недействителен на дату подписи",
                _ => "Timestamp certificate was not valid at the date of signing"
            },
            EImzoStatusCode.CertificatePolicyDisallowed => lang switch
            {
                "uz" => "Sertifikat siyosati ruxsat etilganlar ro'yxatiga kirmaydi",
                "ru" => "Политика сертификата не входит в разрешённый набор",
                _ => "Certificate policy is not in the allowed set"
            },
            EImzoStatusCode.CaCertificateDisallowed => lang switch
            {
                "uz" => "RTI (CA) sertifikati tekshirish uchun ruxsat etilmagan",
                "ru" => "Сертификат УЦ не разрешён для проверки",
                _ => "Certification Authority (CA) certificate is not allowed for verification"
            },
            _ => lang switch
            {
                "uz" => $"Noma'lum xatolik [Kod: {(int)code}]",
                "ru" => $"Неизвестная ошибка [Код: {(int)code}]",
                _ => $"Unknown error [Code: {(int)code}]"
            }
        };
    }

    /// <summary>
    /// Gets a human-readable description for an <see cref="EImzoMobileStatusCode"/> in the requested language (uz, ru, or en).
    /// </summary>
    public static string GetDescription(this EImzoMobileStatusCode code, string language = "en")
    {
        var lang = (language ?? "en").ToLowerInvariant();
        return code switch
        {
            EImzoMobileStatusCode.Success => lang switch
            {
                "uz" => "Muvaffaqiyatli",
                "ru" => "Успешно",
                _ => "Success"
            },
            EImzoMobileStatusCode.PendingUpload => lang switch
            {
                "uz" => "PKCS#7 hali mobil ilova tomonidan yuklanmagan (kutilmoqda)",
                "ru" => "PKCS#7 еще не загружен со стороны ИС ID-CARD E-IMZO MOBILE",
                _ => "PKCS#7 has not yet been uploaded from the mobile application (pending)"
            },
            EImzoMobileStatusCode.RedisError => lang switch
            {
                "uz" => "Redis bilan bog'liq xatolik yuz berdi",
                "ru" => "Ошибка связанная с Redis (возможно к нему не удалось подключиться)",
                _ => "Redis storage error or connection failure"
            },
            EImzoMobileStatusCode.DocumentNotFound => lang switch
            {
                "uz" => "DocumentID bo'yicha yozuv topilmadi (muddati tugagan bo'lishi mumkin)",
                "ru" => "Запись по DocumentID не найдена в Redis (возможно истекло время жизни)",
                _ => "Record by DocumentID was not found (may have expired)"
            },
            EImzoMobileStatusCode.InvalidPkcs7Structure => lang switch
            {
                "uz" => "PKCS#7 hujjati strukturasi yaroqsiz",
                "ru" => "Структура PKCS#7 документа недействительна",
                _ => "PKCS#7 structure is invalid or absent"
            },
            EImzoMobileStatusCode.SignatureInvalid => lang switch
            {
                "uz" => "PKCS#7 hujjati ERI imzosi yaroqsiz",
                "ru" => "ЭЦП PKCS#7 документа недействительна",
                _ => "PKCS#7 digital signature is invalid"
            },
            EImzoMobileStatusCode.CertificateInvalid => lang switch
            {
                "uz" => "Foydalanuvchi sertifikati haqiqiy emas",
                "ru" => "Сертификат пользователя недействителен",
                _ => "User certificate is invalid"
            },
            EImzoMobileStatusCode.CertificateInvalidAtSigningTime => lang switch
            {
                "uz" => "Sertifikat imzolangan vaqtda haqiqiy bo'lmagan",
                "ru" => "Сертификат пользователя недействителен на дату и время подписи",
                _ => "User certificate was not valid at the date and time of signing"
            },
            EImzoMobileStatusCode.CertificateStatusCheckError => lang switch
            {
                "uz" => "Sertifikat holatini tekshirishda xatolik yuz berdi",
                "ru" => "Произошла ошибка при проверке статуса сертификата",
                _ => "Error occurred while checking certificate status"
            },
            EImzoMobileStatusCode.TimeWindowExceeded => lang switch
            {
                "uz" => "Smartfondagi imzolash vaqti bilan server vaqti o'rtasidagi farq oshib ketdi",
                "ru" => "Превышена разрешенная разность времени между смартфоном и сервером",
                _ => "Allowed time difference between smartphone and server was exceeded"
            },
            EImzoMobileStatusCode.UnexpectedError => lang switch
            {
                "uz" => "Kutilmagan server xatoligi",
                "ru" => "Непредвиденная ошибка E-IMZO-SERVER",
                _ => "Unexpected server error"
            },
            EImzoMobileStatusCode.DigestMismatchOrGeneralError => lang switch
            {
                "uz" => "Hesh mos kelmadi: hisoblangan xesh signerInfo heshiga to'g'ri kelmadi",
                "ru" => "Хеш документа не совпадает с хешем внутри PKCS#7 (Calculated digest mismatch)",
                _ => "Calculated digest for document does not match signerInfo digest"
            },
            _ => lang switch
            {
                "uz" => $"Noma'lum mobil holat kodi [Kod: {(int)code}]",
                "ru" => $"Неизвестный статус [Код: {(int)code}]",
                _ => $"Unknown mobile status code [Code: {(int)code}]"
            }
        };
    }
}
