# Application — Regras para o Agente

> Leia `backend/AGENTS.md` antes deste arquivo.

## RESPONSABILIDADE

Definir contratos (interfaces) e objetos de transferencia (DTOs).
Camada de abstracao entre Domain e Infrastructure.

## ESTRUTURA

```
Application/
├── DTOs/         ← objetos de entrada (Request) e saida (Response)
├── Interfaces/   ← contratos de repositorios e servicos
└── Mappings/     ← AutoMapper profiles (quando adicionado)
```

## ARQUIVOS EXISTENTES

| Tipo | Arquivo | Descricao |
|------|---------|-----------|
| DTO | `DTOs/ExampleRequestDto.cs` | Entrada para criar/atualizar Example |
| DTO | `DTOs/ExampleResponseDto.cs` | Saida da API para Example |
| Interface | `Interfaces/IExampleRepository.cs` | Contrato do repositorio de Example |
| Interface | `Interfaces/IExampleService.cs` | Contrato do servico de Example (legado) |

## REGRAS OBRIGATORIAS — DTOs

- Namespace: `SMCV.Application.DTOs`
- Sufixo `RequestDto` para entrada, `ResponseDto` para saida
- Campos obrigatorios usam `required`
- RequestDto NAO inclui `Id`, `CreatedAt`, `UpdatedAt`
- ResponseDto inclui todos os campos publicos da entidade

## REGRAS OBRIGATORIAS — Interfaces

- Namespace: `SMCV.Application.Interfaces`
- Prefixo `I` no nome (ex: `IExampleRepository`)
- Metodos async com sufixo `Async`
- Repository trabalha com Entity; Service trabalha com DTOs

## PROIBICOES

- **SEM** implementacao concreta nesta camada — apenas contratos
- **SEM** referencia a `DbContext` ou pacotes de infraestrutura
