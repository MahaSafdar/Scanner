# Automated Network Vulnerability Scanner

This C#-based automated vulnerability scanner is designed to streamline network security assessments. Combining network discovery, port scanning, service detection, and vulnerability analysis into a unified desktop application, it simplifies traditionally time-consuming security workflows. Its intuitive graphical interface makes it ideal for small to medium-sized network security evaluations.

---

## 🛠️ Technology Stack

| Layer                  | Technology/Tool | Logo |
|------------------------|----------------|------|
| Programming Language    | C# | <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" height="28"> |
| Framework               | .NET 6+ | <img src="https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=.net&logoColor=white" height="28"> |
| IDE                     | Visual Studio | <img src="https://img.shields.io/badge/Visual_Studio-5C2D91?style=for-the-badge&logo=visual-studio&logoColor=white" height="28"> |
| Network Tools           | Nmap | <img src="https://img.shields.io/badge/Nmap-9A9A9A?style=for-the-badge&logo=nmap&logoColor=white" height="28"> |
| APIs                    | NVD/CVE | <img src="https://img.shields.io/badge/NVD-003366?style=for-the-badge&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAACXBIWXMAAAsTAAALEwEAmpwYAAAARUlEQVQ4je3SQREAMAgD0ed5/1YJ3pJAGm6x2tMYI4C0M1zDcG+jNJzvSDTCqLO+L0C5MwyjLMQxCBsxGI6qNx8y5TgLQXhwh6pZxSIAAAAASUVORK5CYII=" height="28"> |
| Concurrency             | Async/Await | <img src="https://img.shields.io/badge/Async-Await-007ACC?style=for-the-badge&logo=react&logoColor=white" height="28"> |
| GUI                     | WinForms / WPF | <img src="https://img.shields.io/badge/WPF-Windows-512BD4?style=for-the-badge&logo=windows&logoColor=white" height="28"> |
| Data Storage            | JSON/XML, SQLite | <img src="https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite&logoColor=white" height="28"> |

---

## 🚀 Key Features

### 1. Network Discovery
- **ARP Scan**: Identify active hosts on the local network.  
- **ICMP Scan**: Detect hosts using echo requests.  
- **ICMP Fragmentation Scan**: Advanced host discovery using fragmented packets for stealth scanning.

### 2. Port Scanning
- Scan **well-known**, **registered**, **dynamic**, or **custom port ranges**.  
- **Asynchronous multi-threaded scanning** with up to 100 concurrent tasks.  
- Timeout, error-handling, and optimized performance for large networks.

### 3. Service and Version Detection
- **Service fingerprinting** using:  
  - Banner grabbing  
  - Protocol-specific probes  
  - Integrated Nmap service/probe datasets  
- **Version identification** for common protocols like HTTP, SSH, FTP, SMB, and SQL.

### 4. Vulnerability Assessment
- Query the **National Vulnerability Database (NVD API)** for CVE data.  
- Automated **severity classification** and actionable recommendations.  
- Intelligent caching and rate-limited API requests for performance.

### 5. Automated Scanning
- Schedule scans based on user-defined intervals.  
- Automatic host range selection.  
- Auto-generated, archival-ready scan reports.

### 6. Graphical User Interface
- **Dashboard**: Centralized vulnerability insights and summaries.  
- **Scan History**: Access and export past results.  
- **Scan Details**: Deep dive into open ports, services, versions, and vulnerabilities.  
- **New Scan Page**: Manual scan configuration.  
- **Settings**: Theme selection, default scan types, report directory management.
  
---

## 🏗️ System Architecture

### Scanning Engine (`portscanner.sln`)
- **PortScannerImpl** – Core async port scanning engine.  
- **RegisteredPortHandler** – Maps ports to known service fingerprints.  
- **VulnerabilityScanner** – Parses service information and queries CVE data.  
- **VulnerabilityDatabase** – Handles API requests, caching, and parsing.

### GUI Layer (`GradProject.sln`)
- **Dashboard**, **NewScanPage**, **ScanDetails**, **ScanHistory**, **ScannerSettings**, **AutomatedScanner**  
- GUI interacts with the scanning engine via **event-driven updates** and **async callbacks**.

---

## 🎯 Project Scope
This tool provides:
- Network discovery  
- Port and service enumeration  
- Automated vulnerability detection  
- Scheduled scanning and reporting  
- User-friendly GUI

---

## 🔮 Future Enhancements
- AI-powered vulnerability remediation suggestions  
- Multi-format report export: HTML, JSON, PDF  
- Expanded automation workflows  
- Enhanced accessibility modes  
- Customizable UI layouts  

---
---

## Demo
Dashboard: 
<img width="940" height="377" alt="image" src="https://github.com/user-attachments/assets/59c7774c-46b2-483e-b26c-e7dc52a0d3fa" />

Scan History Page:
<img width="940" height="561" alt="image" src="https://github.com/user-attachments/assets/55b142f7-8750-4af3-a77f-4cddbe1fee24" />

Scan Details:
<img width="940" height="709" alt="image" src="https://github.com/user-attachments/assets/02035dd0-aa23-4f1a-8470-800c5ed18ce5" />

Setting:
<img width="940" height="709" alt="image" src="https://github.com/user-attachments/assets/a82b1cbb-9a63-4d53-813d-90546e96d656" />







---

This scanner empowers cybersecurity professionals and enthusiasts to identify and remediate vulnerabilities efficiently, making network security accessible, automated, and effective.
