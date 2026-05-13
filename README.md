# Suprema South Africa — Partner Site

**Domain:** suprema.co.za  
**Stack:** .NET 10 / ASP.NET Core MVC  
**Owner:** Tshepo Tlou  
**Status:** v0.1-scaffold — PLACEHOLDER copy throughout, pending approved Suprema partner kit.

> Read [`SUPREMA_ZA_BUILD_PLAN.md`](./SUPREMA_ZA_BUILD_PLAN.md) before touching any code.  
> It is the single source of truth for architecture, conventions, and the remaining build plan.

---

## Running locally

```bash
# Prerequisites: .NET 10 SDK
git clone https://github.com/neaMetricsv2/suprema.git
cd suprema/src/Suprema.Web
dotnet run
# → http://localhost:5030
```

You should see startup output like:
```
info: Suprema.Content.Services.ContentService[0]
      Content loaded — categories: 3, products: 7, solutions: 5, articles: 1, pages: 5
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5030
```

---

## Project structure

```
suprema/
├── SUPREMA_ZA_BUILD_PLAN.md   ← read this first
├── src/
│   ├── Suprema.sln
│   ├── Suprema.Core/          ← models + interfaces (no dependencies)
│   ├── Suprema.Content/       ← content loaders + Data/ files (JSON + Markdown)
│   └── Suprema.Web/           ← ASP.NET Core MVC app
```

### Adding content

**New product** — create `src/Suprema.Content/Data/products/{category-slug}/{product-slug}.json`  
**New solution** — create `src/Suprema.Content/Data/solutions/{slug}.json`  
**New article** — create `src/Suprema.Content/Data/articles/{slug}.json`  
**New page** — create `src/Suprema.Content/Data/pages/{slug}.md` with YAML front matter

Duplicate slugs throw at startup — fix before merging.

### Replacing PLACEHOLDER copy

Search for `PLACEHOLDER` across `src/Suprema.Content/Data/` and replace with  
approved content from the Suprema partner asset kit. **Do not paraphrase the  
global site** — use only the approved kit.

### Replacing PLACEHOLDER images

Drop approved assets into `src/Suprema.Web/wwwroot/assets/` following the  
paths referenced in each JSON file (e.g. `/assets/products/xstation-2/hero.jpg`).

---

## Legal (POPIA)

All four legal pages under `Data/pages/` are marked **DRAFT — REVIEW REQUIRED**.  
They must be reviewed by legal counsel before the site goes live.  
See §11 and §17 of the build plan for open questions (Information Officer, VAT, reg. no.).

---

## Phase 4 — CMS migration path

See §16 of the build plan.  
Short version: implement `IContentService` as `UmbracoContentService`;  
controllers and views don't change.

---

## Open questions for Tshepo

See §17 of the build plan — Information Officer, entity legal details,  
hosting environment, DNS, email provisioning, case study permissions.
