# Automated Network Vulnerability Scanner

ProScanner is a C#-based automated vulnerability scanner designed to streamline network security assessments. It integrates port scanning, service detection, and vulnerability analysis into a unified desktop application with an intuitive graphical interface. The tool automates traditionally manual and time-consuming scanning workflows, enabling efficient security evaluation for small and medium-sized networks.

---

## Features

### 1. Network Discovery
- ARP Scan to identify active hosts in the local network.
- ICMP Scan for host detection using echo requests.
- ICMP Fragmentation Scan for advanced host discovery using fragmented packets.

### 2. Port Scanning
- Scan well-known, registered, dynamic, or custom port ranges.
- Asynchronous multi-threaded scanning (up to 100 concurrent tasks).
- Timeout, error-handling, and performance-optimized scanning pipeline.

### 3. Service and Version Detection
- Service fingerprinting via:
  - Banner grabbing  
  - Protocol-specific probes  
  - Integrated Nmap service and probe datasets
- Version identification for common protocols (HTTP, SSH, FTP, SMB, SQL services).

### 4. Vulnerability Assessment
- CVE and vulnerability lookup via the National Vulnerability Database (NVD) API.
- Severity classification and recommendation generation.
- Intelligent caching and rate-limited API requests.

### 5. Automated Scanning
- Scheduled scans based on user-defined intervals.
- Automatic host range selection.
- Automated report generation and archival.

### 6. Graphical User Interface
Includes:
- **Dashboard** for centralized summaries and vulnerability insights.
- **Scan History** for accessing and exporting past results.
- **Scan Details** for full data on open ports, services, versions, and vulnerabilities.
- **New Scan** page for manual scan configuration.
- **Settings** page for theme selection, default scan types, and report directory control.

---

## Technology Stack
- **Language:** C#
- **Framework:** .NET
- **IDE:** Microsoft Visual Studio
- **Integrations & Tools:**  
  - Nmap service probes  
  - NVD/CVE API  
  - Packet capture libraries for ICMP/ARP  
  - Async/await concurrency model  

---

## System Architecture

### portscanner.sln (Scanning Engine)
Key components:
- **PortScannerImpl** – Core asynchronous port scanning logic.
- **RegisteredPortHandler** – Maps ports to known service fingerprints.
- **VulnerabilityScanner** – Parses service info and queries vulnerabilities.
- **VulnerabilityDatabase** – Handles API calls, caching, and CVE parsing.

### GradProject.sln (GUI Layer)
Primary UI classes:
- Dashboard  
- NewScanPage  
- ScanDetails  
- ScanHistory  
- ScannerSettings  
- AutomatedScanner  

The GUI interacts with the scanning engine through event-driven updates and asynchronous callbacks.

---

## Project Scope

ProScanner provides:
- Network discovery  
- Port and service enumeration  
- Vulnerability detection  
- Automated scanning and reporting  
- Fully functional GUI  

---

## Future Enhancements
- AI-powered vulnerability remediation suggestions.
- Expanded automation workflows.
- Multi-format report export (HTML, JSON, PDF).
- Enhanced accessibility modes.
- Customizable UI layouts.

