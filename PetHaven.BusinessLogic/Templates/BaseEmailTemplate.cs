
namespace PetHaven.BusinessLogic.Templates
{
    public static class BaseEmailTemplate
    {
        public static string GetBaseTemplate(string content, string buttonUrl = null, string buttonText = null)
        {
            var buttonHtml = !string.IsNullOrEmpty(buttonUrl) && !string.IsNullOrEmpty(buttonText)
                ? $@"<table border='0' cellpadding='0' cellspacing='0' class='btn btn-primary' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%; box-sizing: border-box;'>
                    <tbody>
                      <tr>
                        <td align='center' style='font-family: sans-serif; font-size: 14px; vertical-align: top; padding-bottom: 15px;'>
                          <table border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;'>
                            <tbody>
                              <tr>
                                <td style='font-family: sans-serif; font-size: 14px; vertical-align: top; background-color: #1a9b9b; border-radius: 6px; text-align: center;'> 
                                  <a href='{buttonUrl}' target='_blank' style='display: inline-block; color: #ffffff; background-color: #1a9b9b; border: solid 1px #1a9b9b; border-radius: 6px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 14px; font-weight: bold; margin: 0; padding: 12px 25px; text-transform: capitalize;'>{buttonText}</a> 
                                </td>
                              </tr>
                            </tbody>
                          </table>
                        </td>
                      </tr>
                    </tbody>
                  </table>"
                : "";

            return $@"
<!DOCTYPE html>
<html>
<head>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
  <title>PetHaven</title>
  <style>
    @media only screen and (max-width: 620px) {{
      table.body h1 {{
        font-size: 28px !important;
        margin-bottom: 10px !important;
      }}
      table.body p,
      table.body ul,
      table.body ol,
      table.body td,
      table.body span,
      table.body a {{
        font-size: 16px !important;
      }}
      table.body .wrapper,
      table.body .article {{
        padding: 10px !important;
      }}
      table.body .content {{
        padding: 0 !important;
      }}
      table.body .container {{
        padding: 0 !important;
        width: 100% !important;
      }}
      table.body .main {{
        border-left-width: 0 !important;
        border-radius: 0 !important;
        border-right-width: 0 !important;
      }}
      table.body .btn table {{
        width: 100% !important;
      }}
      table.body .btn a {{
        width: 100% !important;
      }}
    }}
    @media all {{
      .ExternalClass {{
        width: 100%;
      }}
      .ExternalClass,
      .ExternalClass p,
      .ExternalClass span,
      .ExternalClass font,
      .ExternalClass td,
      .ExternalClass div {{
        line-height: 100%;
      }}
      #MessageViewBody a {{
        color: inherit;
        text-decoration: none;
        font-size: inherit;
        font-family: inherit;
        font-weight: inherit;
        line-height: inherit;
      }}
    }}
  </style>
</head>
<body style='background-color: #f8f9fa; font-family: sans-serif; -webkit-font-smoothing: antialiased; font-size: 14px; line-height: 1.4; margin: 0; padding: 0; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;'>
  <table role='presentation' border='0' cellpadding='0' cellspacing='0' class='body' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #f8f9fa; width: 100%;' width='100%' bgcolor='#f8f9fa'>
    <tr>
      <td style='font-family: sans-serif; font-size: 14px; vertical-align: top;' valign='top'>&nbsp;</td>
      <td class='container' style='font-family: sans-serif; font-size: 14px; vertical-align: top; display: block; max-width: 580px; padding: 10px; width: 580px; margin: 0 auto;' width='580' valign='top'>
        <div class='header' style='padding: 20px 0; text-align: center;'>
          <img src='https://firebasestorage.googleapis.com/v0/b/pethaven-5b27b.firebasestorage.app/o/images%2FPethaven_logo.png?alt=media&token=fe05e7d5-f36f-459d-88f2-31480d8dd969' width='200' height='60' alt='PetHaven Logo' style='border: none; -ms-interpolation-mode: bicubic; max-width: 100%; display: block; margin: 0 auto;'>
        </div>
        <div class='content' style='box-sizing: border-box; display: block; margin: 0 auto; max-width: 580px; padding: 10px;'>
          <table role='presentation' class='main' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; background: #ffffff; border-radius: 12px; width: 100%; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.06);' width='100%'>
            <tr>
              <td class='wrapper' style='font-family: sans-serif; font-size: 14px; vertical-align: top; box-sizing: border-box; padding: 20px;' valign='top'>
                <table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;' width='100%'>
                  <tr>
                    <td style='font-family: sans-serif; font-size: 14px; vertical-align: top;' valign='top'>
                      {content}
                      {buttonHtml}
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
          <div class='footer' style='clear: both; margin-top: 10px; text-align: center; width: 100%;'>
            <table role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: 100%;' width='100%'>
              <tr>
                <td class='content-block' style='font-family: sans-serif; vertical-align: top; padding-bottom: 10px; padding-top: 10px; color: #6c757d; font-size: 12px; text-align: center;' valign='top' align='center'>
                  <p style='font-family: sans-serif; font-size: 12px; color: #6c757d; margin: 0; margin-bottom: 5px;'>© 2025 PetHaven. All rights reserved.</p>
                  <p style='font-family: sans-serif; font-size: 12px; color: #6c757d; margin: 0;'>
                    <a href='#' style='text-decoration: underline; color: #6c757d; font-size: 12px; text-align: center;'>Privacy Policy</a> |
                    <a href='#' style='text-decoration: underline; color: #6c757d; font-size: 12px; text-align: center;'>Terms of Service</a> |
                    <a href='#' style='text-decoration: underline; color: #6c757d; font-size: 12px; text-align: center;'>Unsubscribe</a>
                  </p>
                </td>
              </tr>
            </table>
          </div>
        </div>
      </td>
      <td style='font-family: sans-serif; font-size: 14px; vertical-align: top;' valign='top'>&nbsp;</td>
    </tr>
  </table>
</body>
</html>
            ";
        }
    }
}
