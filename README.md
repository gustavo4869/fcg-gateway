# FIAP Cloud Games - Gateway

> API Gateway para o sistema de microsserviços da FIAP Cloud Games

Gateway desenvolvido com YARP (Yet Another Reverse Proxy) para .NET 10, responsável por rotear requisições para os microsserviços de Usuários, Jogos e Pagamentos.

[![Deploy Status](https://github.com/gustavo4869/fcg-gateway/actions/workflows/azure-deploy.yml/badge.svg)](https://github.com/gustavo4869/fcg-gateway/actions)

**?? URL Pública**: https://ca-fcg-gateway.whitegrass-9df3aed8.brazilsouth.azurecontainerapps.io

## ?? Quick Start

**Novo no projeto?** Siga o guia rápido: [QUICK_START.md](QUICK_START.md)

**Precisa dos comandos?** Veja: [COMMANDS.md](COMMANDS.md)

## ?? Sumário

- [Visão Geral](#-visão-geral)
- [Arquitetura](#-arquitetura)
- [Funcionalidades](#-funcionalidades)
- [Tecnologias](#-tecnologias)
- [Ambiente Azure](#-ambiente-azure)
- [Instalação e Execução](#-instalação-e-execução)
- [Rotas](#-rotas)
- [Observabilidade](#-observabilidade)
- [CI/CD](#-cicd)
- [Documentação](#-documentação)

## ?? Visão Geral

O FCG Gateway é um API Gateway que centraliza o acesso aos microsserviços do sistema FIAP Cloud Games. Ele é responsável por:

- **Roteamento**: Direcionar requisições para os microsserviços apropriados
- **Observabilidade**: Logs estruturados e telemetria com Application Insights
- **Correlação**: Rastreamento de requisições entre serviços (Distributed Tracing)
- **Health Checks**: Monitoramento da saúde do gateway

## ??? Arquitetura

O gateway atua como ponto de entrada único para os seguintes microsserviços:

- **Users Service** (`/users/*`) - Gerenciamento de usuários e autenticação
- **Games Service** (`/games/*`) - Catálogo e busca de jogos
- **Payments Service** (`/payments/*`) - Processamento de pagamentos

```
Cliente ? Gateway ? Microsserviços
          ?
    Observabilidade
    (Logs + Telemetria)
```

## ? Funcionalidades

### Roteamento Inteligente
- ? Remoção automática de prefixos de rota
- ? Balanceamento de carga (quando múltiplos destinos)
- ? Suporte a HTTP/1.1 e HTTP/2
- ? Path-based routing

### Observabilidade
- ? **Logs Estruturados**: Logs detalhados de todas as requisições
- ? **Application Insights**: Telemetria completa para Azure
- ? **Correlation ID**: Rastreamento distribuído entre microsserviços
- ? **Health Checks**: Endpoints de saúde (`/health`, `/health/ready`, `/health/live`)

### Resiliência
- ? Timeout configurável
- ? Health probes (liveness + readiness)
- ?? Retry policies (pode ser configurado)
- ?? Circuit breaker (pode ser configurado)

## ??? Tecnologias

| Tecnologia | Versão | Descrição |
|-----------|--------|-----------|
| .NET | 10.0 | Framework |
| YARP | 2.3.0 | Reverse Proxy |
| Application Insights | 2.22.0 | Telemetria |
| Azure Container Apps | - | Hosting Serverless |
| GitHub Actions | - | CI/CD |
| Docker | - | Containerização |

## ?? Ambiente Azure

O gateway está deployado no Azure com a seguinte configuração:

| Recurso | Nome/Valor |
|---------|-----------|
| **Resource Group** | `fiap-cloud-games-rg` |
| **Container App** | `ca-fcg-gateway` |
| **Environment** | `cae-fcg-g4869` |
| **Log Analytics** | `law-fcg-g4869` |
| **Região** | `Brazil South` |
| **URL Pública** | `https://ca-fcg-gateway.whitegrass-9df3aed8.brazilsouth.azurecontainerapps.io` |

## ?? Instalação e Execução

### Executar Localmente

1. Clone o repositório:
```bash
git clone https://github.com/gustavo4869/fcg-gateway.git
cd fcg-gateway
```

2. Restaure as dependências:
```bash
dotnet restore
```

3. Execute a aplicação:
```bash
cd Fcg.Gateway
dotnet run
```

O gateway estará disponível em `http://localhost:8080`.

### Executar com Docker

```bash
docker build -f Fcg.Gateway/Dockerfile -t fcg-gateway:local .
docker run -p 8080:8080 fcg-gateway:local
```

### Deploy no Azure

Veja o guia completo: [QUICK_START.md](QUICK_START.md)

Ou use os comandos prontos: [COMMANDS.md](COMMANDS.md)

## ?? Rotas

### Microsserviço de Usuários
```
/users/*  ? https://ca-fcg-users.whitegrass-9df3aed8.brazilsouth.azurecontainerapps.io
```

Exemplos:
```bash
# Criar usuário
POST https://ca-fcg-gateway.../users/api/v1/usuarios

# Login
POST https://ca-fcg-gateway.../users/api/v1/auth/login

# Listar usuários (Admin)
GET https://ca-fcg-gateway.../users/api/v1/usuarios
```

### Microsserviço de Jogos
```
/games/*  ? https://ca-fcg-games.whitegrass-9df3aed8.brazilsouth.azurecontainerapps.io
```

Exemplos:
```bash
# Listar jogos
GET https://ca-fcg-gateway.../games/api/v1/jogos

# Buscar jogo
GET https://ca-fcg-gateway.../games/api/v1/jogos/{id}
```

### Microsserviço de Pagamentos
```
/payments/*  ? https://ca-fcg-payments.whitegrass-9df3aed8.brazilsouth.azurecontainerapps.io
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
GET /                ? Informações do gateway
GET /health          ? Health check geral
GET /health/ready    ? Readiness probe
GET /health/live     ? Liveness probe
```

## ?? Observabilidade

### Application Insights

O gateway está integrado com Azure Application Insights para telemetria completa:

- **Requisições**: Todas as requisições HTTP são rastreadas
- **Dependências**: Chamadas para microsserviços são monitoradas
- **Exceções**: Erros são capturados e enviados automaticamente
- **Métricas**: Performance e uso de recursos

### Correlation ID

Cada requisição recebe ou propaga um `X-Correlation-ID` header para rastreamento distribuído:

```bash
curl -H "X-Correlation-ID: 12345678-1234-1234-1234-123456789012" \
  https://ca-fcg-gateway.whitegrass-9df3aed8.brazilsouth.azurecontainerapps.io/health
```

### Logs Estruturados

Exemplo de log de requisição:
```
[Information] Gateway request started: GET /users/123
[Information] Gateway request completed: GET /users/123 - Status: 200 - Duration: 45.2ms
```

### Health Checks

| Endpoint | Descrição | Uso |
|----------|-----------|-----|
| `/health` | Status geral | Monitoramento |
| `/health/ready` | Readiness probe | Azure Container Apps |
| `/health/live` | Liveness probe | Azure Container Apps |

Teste:
```bash
curl https://ca-fcg-gateway.whitegrass-9df3aed8.brazilsouth.azurecontainerapps.io/health
```

## ?? CI/CD

O projeto utiliza GitHub Actions para CI/CD com deploy automático no Azure Container Apps.

### Pipeline

1. **Build and Test**: Compila e testa o código
2. **Build and Push Image**: Cria imagem Docker e envia para Azure Container Registry (`crfcg.azurecr.io`)
3. **Deploy**: Atualiza o Container App no Azure

### Secrets Necessários

Veja o guia completo: [GITHUB_SECRETS.md](GITHUB_SECRETS.md)

Configure os seguintes secrets no GitHub:

- `AZURE_CREDENTIALS`: Credenciais de Service Principal
- `AZURE_REGISTRY_USERNAME`: Usuário do Container Registry
- `AZURE_REGISTRY_PASSWORD`: Senha do Container Registry

### Deploy Manual

```bash
az containerapp update \
  --name ca-fcg-gateway \
  --resource-group fiap-cloud-games-rg \
  --image crfcg.azurecr.io/fcg-gateway:latest
```

## ?? Documentação

| Documento | Descrição |
|-----------|-----------|
| [QUICK_START.md](QUICK_START.md) | Guia rápido de deploy (20-25 min) |
| [COMMANDS.md](COMMANDS.md) | Comandos prontos para copiar e colar |
| [GITHUB_SECRETS.md](GITHUB_SECRETS.md) | Configuração de secrets do GitHub |
| [AZURE_SETUP.md](AZURE_SETUP.md) | Setup completo do Azure |
| [EXAMPLES.md](EXAMPLES.md) | Exemplos de uso da API |
| [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md) | Resumo técnico da implementação |
| [CHANGES_SUMMARY.md](CHANGES_SUMMARY.md) | Resumo das alterações |

## ?? Troubleshooting

### Gateway não consegue acessar microsserviços

Verifique:
1. Os microsserviços estão rodando
2. Os Container Apps estão no mesmo environment
3. As configurações de ingress permitem comunicação interna

```bash
# Listar todos os Container Apps
az containerapp list \
  --resource-group fiap-cloud-games-rg \
  --query "[].{Name:name, Status:properties.runningStatus}" \
  --output table
```

### Application Insights não está recebendo telemetria

Verifique:
1. A connection string está configurada
2. O secret está no Container App
3. Aguarde 2-3 minutos após fazer requisições

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

## ?? Tech Challenge - Requisitos Atendidos

| Requisito | Status | Implementação |
|-----------|--------|---------------|
| API Gateway | ? | YARP com roteamento para 3 microsserviços |
| Microsserviços | ? | Users, Games, Payments |
| Serverless | ? | Azure Container Apps |
| Event Sourcing | ? | Integrado com microsserviços |
| Observabilidade | ? | Logs + Application Insights + Traces |
| CI/CD | ? | GitHub Actions |
| Segurança | ? | Secrets management + Correlation ID |

## ?? Licença

Projeto acadêmico - FIAP Cloud Games

## ?? Contato

FIAP Cloud Games Team

- **Repositório**: https://github.com/gustavo4869/fcg-gateway
- **Issues**: https://github.com/gustavo4869/fcg-gateway/issues
- **Actions**: https://github.com/gustavo4869/fcg-gateway/actions

---

Desenvolvido com ?? usando .NET 10 e YARP para o Tech Challenge - FIAP

**Status**: ? Pronto para produção
