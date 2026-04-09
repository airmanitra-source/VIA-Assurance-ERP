# Deploiement AWS - VIA Assurance ERP

## Stack utilisee
- **App** : AWS Elastic Beanstalk (.NET on Linux)
- **Base de donnees** : AWS RDS (SQL Server)
- **Secrets** : Variables d'environnement EB

---

## Prerequis

| Outil | Commande / Lien |
|---|---|
| AWS CLI | https://aws.amazon.com/cli/ |
| EB CLI | `pip install awsebcli` |
| .NET 10 SDK | https://dotnet.microsoft.com/download |

---

## 1. Creer la base de donnees (RDS SQL Server)

1. AWS Console > **RDS** > **Create database**
2. Engine : **SQL Server Express** (gratuit jusqu'a 10 GB)
3. DB instance identifier : `via-assurance-db`
4. Public access : `No`
5. Security Group : autoriser le port **1433** depuis le SG de l'EB
6. Notez l'endpoint RDS

### Appliquer les migrations

Connectez-vous a l'instance RDS et executez dans l'ordre :

```
FileTable.Infrastructure\FileTableDb\Migrations\20260224_add_email_to_employee.sql
FileTable.Infrastructure\FileTableDb\Migrations\20260225_add_initial_password_reset_completed_to_appuser.sql
```

---

## 2. Creer l'environnement Elastic Beanstalk

```bash
eb init via-assurance-erp --platform "dotnet-on-linux" --region eu-west-1
eb create via-assurance-prod
```

Ou via la **AWS Console** :
- Elastic Beanstalk > Create Application
- Platform : `.NET on Linux` / `.NET 10`
- Environment : `via-assurance-prod`

---

## 3. Variables d'environnement EB

EB Console > Configuration > Software > Environment properties :

| Cle | Valeur |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__FileTableConnection` | `Server=<RDS_ENDPOINT>,1433;Database=via_assurance;User Id=<USER>;Password=<PWD>;Encrypt=True;TrustServerCertificate=True;` |
| `Email__SmtpServer` | `smtp.gmail.com` |
| `Email__SmtpPort` | `587` |
| `Email__SenderEmail` | `no-reply@your-domain.com` |
| `Email__SenderPassword` | `<APP_PASSWORD_GMAIL>` |
| `Email__SenderName` | `VIA Assurance` |
| `UserDefaults__DefaultPassword` | `<MOT_DE_PASSE_PAR_DEFAUT>` |

> Ne jamais mettre de mots de passe dans appsettings.json en production.

---

## 4. Deployer

```powershell
.\deploy-eb.ps1
```

Ou manuellement :

```powershell
dotnet publish ClientApp/ClientApp.csproj -c Release -o ./publish
Compress-Archive -Path ./publish/* -DestinationPath ./deploy.zip -Force
eb deploy via-assurance-prod
```

---

## 5. Verifications post-deploiement

- [ ] https://<EB_URL>/login accessible
- [ ] Login fonctionne
- [ ] Creation utilisateur envoie un email
- [ ] Reset password fonctionne
- [ ] InitialPasswordResetCompleted passe a true

---

## Architecture

```
Internet -> [Elastic Beanstalk .NET 10 / Blazor Server] -> [RDS SQL Server]
```

---

## Cout estime (eu-west-1)

| Service | Instance | Cout/mois |
|---|---|---|
| Elastic Beanstalk (EC2) | t3.small | ~$15 |
| RDS SQL Server Express | db.t3.micro | ~$25 |
| **Total** | | **~$40/mois** |

---

## Liens utiles

- EB CLI : https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/eb-cli3.html
- RDS SQL Server : https://docs.aws.amazon.com/AmazonRDS/latest/UserGuide/CHAP_SQLServer.html
- .NET on EB : https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/create_deploy_NET.html
