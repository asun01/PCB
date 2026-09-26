# 3D SPI Feature Catalog

| Feature ID | Feature | Required input | Output class |
|---|---|---|---|
| 03D-SPI-F001 | PastePresence | Device-native qualified input | Result/Finding/Measurement |
| 03D-SPI-F002 | PasteArea | Device-native qualified input | Result/Finding/Measurement |
| 03D-SPI-F003 | PasteHeight | Device-native qualified input | Result/Finding/Measurement |
| 03D-SPI-F004 | PasteVolume | Device-native qualified input | Result/Finding/Measurement |
| 03D-SPI-F005 | PasteOffset | Device-native qualified input | Result/Finding/Measurement |
| 03D-SPI-F006 | PasteShape | Device-native qualified input | Result/Finding/Measurement |
| 03D-SPI-F007 | PasteCoplanarity | Device-native qualified input | Result/Finding/Measurement |
| 03D-SPI-F008 | BridgingRisk | Device-native qualified input | Result/Finding/Measurement |

## Feature execution contract

Each feature binds an inspection object, ROI or geometry, coordinate frame, algorithm revision, parameters, threshold source, outcome semantics, evidence references, replay inputs, and Quality contribution.

## Decision semantics

PASS, FAIL, REVIEW, INCONCLUSIVE and NOT_EVALUATED are distinct concepts where the authoritative result contract supports them. A missing measurement is not a zero value. An unavailable feature is not a passing feature.

## Parameter authority

Parameters affecting production decisions require an identified authoritative source and revision. Device defaults may be displayed as descriptive defaults but cannot silently become production acceptance limits.