# Security Policy

## Supported Versions

Only the latest release on the `main` branch receives security fixes.

| Version | Supported |
|---------|-----------|
| latest  | ✓ |
| older   | ✗ |

## Reporting a Vulnerability

**Please do not file public GitHub issues for security vulnerabilities.**

Use [GitHub's private vulnerability reporting](https://github.com/rbaker5/xs-pub/security/advisories/new) to submit a report. This keeps details confidential until a fix is available.

Include in your report:
- A description of the vulnerability and its potential impact
- Steps to reproduce (schema or WSDL input that triggers the issue, if applicable)
- Any suggested remediation, if you have one

### What to expect

| Step | Timeline |
|------|----------|
| Acknowledgement | Within 7 days |
| Assessment and triage | Within 14 days |
| Fix or workaround | Best effort; severity-dependent |
| Public disclosure | Coordinated with reporter |

## Scope

Likely in scope: path traversal or SSRF via crafted `xs:import`/`xs:include` schema locations, malformed XSD/WSDL inputs that cause unsafe behavior.

Out of scope: denial-of-service via intentionally large inputs, issues in third-party dependencies (report those upstream).
