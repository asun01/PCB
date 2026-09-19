# Phase 1 — 600→700 Stage Ledger

> Stages 601–700 completed on 2026-09-19. Focus: bounded Replay Bundle diagnostic windows, audit/evidence reference closure, deterministic slicing, JSON roundtrip, nested-window containment, and tamper detection.

## 601-610 — Window boundary model

- [x] 601. Add window descriptor
- [x] 602. Track input count
- [x] 603. Track evidence count
- [x] 604. Track audit count
- [x] 605. Track first input sequence
- [x] 606. Track last input sequence
- [x] 607. Track first evidence generation
- [x] 608. Track last evidence generation
- [x] 609. Track first audit sequence
- [x] 610. Track last audit sequence

## 611-620 — Tail projection semantics

- [x] 611. Take last inputs
- [x] 612. Take last audits
- [x] 613. Retain referenced evidence
- [x] 614. Retain recent evidence
- [x] 615. Order evidence monotonically
- [x] 616. Reject negative input limit
- [x] 617. Reject negative evidence limit
- [x] 618. Reject negative audit limit
- [x] 619. Reject impossible evidence closure
- [x] 620. Rebuild a fresh manifest

## 621-630 — Window validation

- [x] 621. Validate input ordering
- [x] 622. Validate evidence ordering
- [x] 623. Validate audit ordering
- [x] 624. Validate evidence references
- [x] 625. Validate manifest counts
- [x] 626. Validate input hash
- [x] 627. Validate evidence hash
- [x] 628. Validate audit hash
- [x] 629. Validate session hash
- [x] 630. Preserve valid empty windows

## 631-640 — Containment semantics

- [x] 631. Verify source containment
- [x] 632. Verify input containment
- [x] 633. Verify evidence containment
- [x] 634. Verify audit containment
- [x] 635. Verify nested window containment
- [x] 636. Verify nested input bounds
- [x] 637. Verify nested audit bounds
- [x] 638. Verify nested evidence bounds
- [x] 639. Preserve session identity
- [x] 640. Preserve source-only records

## 641-650 — JSON diagnostic windows

- [x] 641. Serialize valid window
- [x] 642. Deserialize valid window
- [x] 643. Preserve manifest
- [x] 644. Preserve inputs
- [x] 645. Preserve evidence
- [x] 646. Preserve audit
- [x] 647. Preserve session hash
- [x] 648. Preserve JSON field names
- [x] 649. Preserve validation state
- [x] 650. Reject invalid serialized state

## 651-660 — Deterministic window reconstruction

- [x] 651. Repeat tail construction
- [x] 652. Repeat manifest reconstruction
- [x] 653. Repeat descriptor generation
- [x] 654. Repeat JSON roundtrip
- [x] 655. Repeat containment checks
- [x] 656. Repeat validation
- [x] 657. Repeat equivalence comparison
- [x] 658. Repeat full-window reconstruction
- [x] 659. Repeat empty-window reconstruction
- [x] 660. Repeat stable hash generation

## 661-670 — Evidence closure behavior

- [x] 661. Audit references retained evidence
- [x] 662. Recent evidence remains available
- [x] 663. Referenced evidence survives pruning
- [x] 664. Evidence remains ordered
- [x] 665. Closure survives JSON roundtrip
- [x] 666. Closure survives nested slicing
- [x] 667. Closure rejects missing evidence
- [x] 668. Closure survives empty state
- [x] 669. Closure preserves StableKey
- [x] 670. Closure remains vendor-neutral

## 671-680 — Tamper detection

- [x] 671. Tampered session hash detected
- [x] 672. Tampered input payload detected
- [x] 673. Tampered evidence hash detected
- [x] 674. Tampered audit payload detected
- [x] 675. Invalid manifest detected
- [x] 676. Invalid count detected
- [x] 677. Invalid ordering detected
- [x] 678. Invalid reference detected
- [x] 679. Invalid window rejected
- [x] 680. Valid window remains equivalent

## 681-690 — 100-round deterministic matrix

- [x] 681. Register the 100-round smoke
- [x] 682. Use ten repeated groups
- [x] 683. Use exactly one Check per group iteration
- [x] 684. Use ten iterations per group
- [x] 685. Cover progressive windows
- [x] 686. Cover full windows
- [x] 687. Cover empty windows
- [x] 688. Cover negative guards
- [x] 689. Cover nested windows
- [x] 690. Cover tampered windows

## 691-700 — Repository hardening

- [x] 691. Register window smoke in main entry
- [x] 692. Add 601–700 ledger
- [x] 693. Update phase progress
- [x] 694. Check changed C# delimiter balance
- [x] 695. Check 100-round Check count
- [x] 696. Check ten loop groups
- [x] 697. Check exact round==100 assertion
- [x] 698. Check branch divergence
- [x] 699. Check workflow/status availability
- [x] 700. Record verification boundary

## Completion status

- [x] Stages 601–700 completed.
- [x] Replay diagnostic Window runtime added.
- [x] Audit→Evidence closure preserved during tail projection.
- [x] Exact 100-round window smoke registered.
- [x] No vendor-specific renderer or HALCON/DevExpress/hardware semantics introduced.
- [x] No build/test success claimed without authoritative execution evidence.
