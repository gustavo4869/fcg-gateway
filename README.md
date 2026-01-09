# FIAP Cloud Games - Gateway

> API Gateway para o sistema de microsserviços da FIAP Cloud Games

Gateway desenvolvido com YARP (Yet Another Reverse Proxy) para .NET 10, responsável por rotear requisições para os microsserviços de Usuários, Jogos e Pagamentos.

[![Deploy Status](https://github.com/gustavo4869/fcg-gateway/actions/workflows/azure-deploy.yml/badge.svg)](https://github.com/gustavo4869/fcg-gateway/actions)

**URL Publica**: https://ca-fcg-gateway.whitegrass-9df3aed8.brazilsouth.azurecontainerapps.io

## Sumario

- [Visao Geral](#visao-geral)
- [Arquitetura](#arquitetura)
- [Funcionalidades](#funcionalidades)
- [Tecnologias](#tecnologias)
- [Ambiente Azure](#ambiente-azure)
- [Instalacao e Execucao](#instalacao-e-execucao)
- [Rotas](#rotas)
- [Observabilidade](#observabilidade)
- [CI/CD](#cicd)
- [Documentacao](#documentacao)

## Visao Geral

O FCG Gateway e um API Gateway que centraliza o acesso aos microsservicos do sistema FIAP Cloud Games. Ele e responsavel por:

- **Roteamento**: Direcionar requisicoes para os microsservicos apropriados
- **Observabilidade**: Logs estruturados e telemetria com Application Insights
- **Correlacao**: Rastreamento de requisicoes entre servicos (Distributed Tracing)
- **Health Checks**: Monitoramento da saude do gateway

## Arquitetura

O gateway atua como ponto de entrada unico para os seguintes microsservicos:

- **Users Service** (`/users/*`) - Gerenciamento de usuarios e autenticacao
- **Games Service** (`/games/*`) - Catalogo e busca de jogos
- **Payments Service** (`/payments/*`) - Processamento de pagamentos

```
Cliente -> Gateway -> Microsservicos
          |
    Observabilidade
    (Logs + Telemetria)
```

## Funcionalidades

### Roteamento Inteligente
- [x] Remocao automatica de prefixos de rota
- [x] Balanceamento de carga (quando multiplos destinos)
- [x] Suporte a HTTP/1.1 e HTTP/2
- [x] Path-based routing

### Observabilidade
- [x] **Logs Estruturados**: Logs detalhados de todas as requisicoes
- [x] **Application Insights**: Telemetria completa para Azure
- [x] **Correlation ID**: Rastreamento distribuido entre microsservicos
- [x] **Health Checks**: Endpoints de saude (`/health`, `/health/ready`, `/health/live`)

### Resiliencia
- [x] Timeout configuravel
- [x] Health probes (liveness + readiness)
- [ ] Retry policies (pode ser configurado)
- [ ] Circuit breaker (pode ser configurado)

## Tecnologias

| Tecnologia | Versao | Descricao |
|-----------|--------|-----------|
| .NET | 10.0 | Framework |
| YARP | 2.3.0 | Reverse Proxy |
| Application Insights | 2.22.0 | Telemetria |
| Azure Container Apps | - | Hosting Serverless |
| GitHub Actions | - | CI/CD |
| Docker | - | Containerizacao |

## Ambiente Azure

O gateway esta deployado no Azure com a seguinte configuracao:

| Recurso | Nome/Valor |
|---------|-----------|
| **Resource Group** | `fiap-cloud-games-rg` |
| **Container App** | `ca-fcg-gateway` |
| **Environment** | `cae-fcg-g4869` |
| **Log Analytics** | `law-fcg-g4869` |
| **Container Registry** | `acrfcggames1222.azurecr.io` |
| **Regiao** | `Brazil South` |
| **URL Publica** | `https://ca-fcg-gateway.whitegrass-9df3aed8.brazilsouth.azurecontainerapps.io` |

## Instalacao e Execucao

### Executar Localmente

1. Clone o repositorio:
```bash
git clone https://github.com/gustavo4869/fcg-gateway.git
cd fcg-gateway
```

2. Restaure as dependencias:
```bash
dotnet restore
```

3. Execute a aplicacao:
```bash
cd Fcg.Gateway
dotnet run
```

O gateway estara disponivel em `http://localhost:8080`.

### Executar com Docker

```bash
docker build -f Fcg.Gateway/Dockerfile -t fcg-gateway:local .
docker run -p 8080:8080 fcg-gateway:local
```

### Deploy no Azure

Veja o guia completo: [QUICK_START.md](QUICK_START.md)

Ou use os comandos prontos: [QUICK_COMMANDS.md](QUICK_COMMANDS.md)

## Rotas

### Microsservico de Usuarios
```
/users/*  -> https://ca-fcg-users.whitegrass-9df3aed8.brazilsouth.azurecontainerapps.io
```

Exemplos:
```bash
# Criar usuario
POST https://ca-fcg-gateway.../users/api/v1/usuarios

# Login
POST https://ca-fcg-gateway.../users/api/v1/auth/login

# Listar usuarios (Admin)
GET https://ca-fcg-gateway.../users/api/v1/usuarios
```

### Microsservico de Jogos
```
/games/*  -> https://ca-fcg-games.whitegrass-9df3aed8.brazilsouth.azurecontainerapps.io
```

Exemplos:
```bash
# Listar jogos
GET https://ca-fcg-gateway.../games/api/v1/jogos

# Buscar jogo
GET https://ca-fcg-gateway.../games/api/v1/jogos/{id}
```

### Microsservico de Pagamentos
```
/payments/*  -> https://ca-fcg-payments.whitegrass-9df3aed8.brazilsouth.azurecontainerapps.io
```

Exemplos:
```bash
# Criar pagamento
POST https://ca-fcg-gateway.../payments/payments

# Buscar pagamento
GET https://ca-fcg-gateway.../payments/payments/{id}
```

### Endpoints do Gateway
```
GET /                -> Informacoes do gateway
GET /health          -> Health check geral
GET /health/ready    -> Readiness probe
GET /health/live     -> Liveness probe
```

## Observabilidade

### Application Insights

O gateway esta integrado com Azure Application Insights para telemetria completa:

- **Requisicoes**: Todas as requisicoes HTTP sao rastreadas
- **Dependencias**: Chamadas para microsservicos sao monitoradas
- **Excecoes**: Erros sao capturados e enviados automaticamente
- **Metricas**: Performance e uso de recursos

### Correlation ID

Cada requisicao recebe ou propaga um `X-Correlation-ID` header para rastreamento distribuido:

```bash
curl -H "X-Correlation-ID: 12345678-1234-1234-1234-123456789012" \
  https://ca-fcg-gateway.whitegrass-9df3aed8.brazilsouth.azurecontainerapps.io/health
```

### Logs Estruturados

Exemplo de log de requisicao:
```
[Information] Gateway request started: GET /users/123
[Information] Gateway request completed: GET /users/123 - Status: 200 - Duration: 45.2ms
```

### Health Checks

| Endpoint | Descricao | Uso |
|----------|-----------|-----|
| `/health` | Status geral | Monitoramento |
| `/health/ready` | Readiness probe | Azure Container Apps |
| `/health/live` | Liveness probe | Azure Container Apps |

Teste:
```bash
curl https://ca-fcg-gateway.whitegrass-9df3aed8.brazilsouth.azurecontainerapps.io/health
```

## CI/CD

O projeto utiliza GitHub Actions para CI/CD com deploy automatico no Azure Container Apps.

### Pipeline

1. **Build and Test**: Compila e testa o codigo
2. **Build and Push Image**: Cria imagem Docker e envia para Azure Container Registry (`acrfcggames1222.azurecr.io`)
3. **Deploy**: Atualiza o Container App no Azure

### Secrets Necessarios

Veja o guia completo: [GITHUB_SECRETS.md](GITHUB_SECRETS.md)

Configure os seguintes secrets no GitHub:

- `AZURE_CREDENTIALS`: Credenciais de Service Principal
- `AZURE_REGISTRY_USERNAME`: Usuario do Container Registry
- `AZURE_REGISTRY_PASSWORD`: Senha do Container Registry

### Deploy Manual

```bash
az containerapp update \
  --name ca-fcg-gateway \
  --resource-group fiap-cloud-games-rg \
  --image acrfcggames1222.azurecr.io/fcg-gateway:latest
```

## Documentacao

| Documento | Descricao |
|-----------|-----------|
| [QUICK_START.md](QUICK_START.md) | Guia rapido de deploy (20-25 min) |
| [QUICK_COMMANDS.md](QUICK_COMMANDS.md) | Comandos prontos para copiar e colar |
| [GITHUB_SECRETS.md](GITHUB_SECRETS.md) | Configuracao de secrets do GitHub |
| [AZURE_SETUP.md](AZURE_SETUP.md) | Setup completo do Azure |
| [EXAMPLES.md](EXAMPLES.md) | Exemplos de uso da API |
| [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md) | Resumo tecnico da implementacao |
| [CHANGES_SUMMARY.md](CHANGES_SUMMARY.md) | Resumo das alteracoes |
| [FIX_SECRET_ERROR.md](FIX_SECRET_ERROR.md) | Solucao para erro de secret |

## Troubleshooting

### Gateway nao consegue acessar microsservicos

Verifique:
1. Os microsservicos estao rodando
2. Os Container Apps estao no mesmo environment
3. As configuracoes de ingress permitem comunicacao interna

```bash
# Listar todos os Container Apps
az containerapp list \
  --resource-group fiap-cloud-games-rg \
  --query "[].{Name:name, Status:properties.runningStatus}" \
  --output table
```

### Application Insights nao esta recebendo telemetria

Verifique:
1. A connection string esta configurada
2. O secret esta no Container App
3. Aguarde 2-3 minutos apos fazer requisicoes

```bash
# Verificar secret
az containerapp secret list \
  --name ca-fcg-gateway \
  --resource-group fiap-cloud-games-rg
```

### Logs de Debug

```bash
# Ver logs em tempo real
az containerapp logs show \
  --name ca-fcg-gateway \
  --resource-group fiap-cloud-games-rg \
  --follow
```

## Tech Challenge - Requisitos Atendidos

| Requisito | Status | Implementacao |
|-----------|--------|---------------|
| API Gateway | [x] | YARP com roteamento para 3 microsservicos |
| Microsservicos | [x] | Users, Games, Payments |
| Serverless | [x] | Azure Container Apps |
| Event Sourcing | [x] | Integrado com microsservicos |
| Observabilidade | [x] | Logs + Application Insights + Traces |
| CI/CD | [x] | GitHub Actions |
| Seguranca | [x] | Secrets management + Correlation ID |

## Licenca

Projeto academico - FIAP Cloud Games

## Contato

FIAP Cloud Games Team

- **Repositorio**: https://github.com/gustavo4869/fcg-gateway
- **Issues**: https://github.com/gustavo4869/fcg-gateway/issues
- **Actions**: https://github.com/gustavo4869/fcg-gateway/actions

---

Desenvolvido com dedicacao usando .NET 10 e YARP para o Tech Challenge - FIAP

**Status**: Pronto para producao
