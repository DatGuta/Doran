﻿using DR.Helper;
using Newtonsoft.Json;

namespace DR.Resource {
    public class Unit {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    public class AddressMap {
        public string Name { get; set; } = string.Empty;
        public Unit? Unit { get; set; }
    }

    internal class AuStorage {
        public string Lv1 { get; set; } = null!;
        public string? Lv2 { get; set; }
        public string? Lv3 { get; set; }
        public string Name { get; set; } = null!;
    }

    public class UnitRes {
        private readonly IReadOnlyList<AuStorage> items = new List<AuStorage>();

        public UnitRes() {
            string filename = Path.Combine("Resources", "administrative_unit.json");
            if (File.Exists(filename)) {
                try {
                    items = JsonConvert.DeserializeObject<List<AuStorage>>(File.ReadAllText(filename)) ?? new List<AuStorage>();
                } finally {
                    items ??= new List<AuStorage>();
                }
            }
        }

        public async Task<List<Unit>> List(string? lv1Code, string? lv2Code) {
            List<Unit> result;
            if (string.IsNullOrWhiteSpace(lv1Code)) {
                result = items.Where(o => !string.IsNullOrWhiteSpace(o.Lv1) && string.IsNullOrWhiteSpace(o.Lv2) && string.IsNullOrWhiteSpace(o.Lv3))
                    .Select(o => new Unit {
                        Code = o.Lv1,
                        Name = o.Name,
                    }).ToList();
                return await Task.FromResult(result);
            }

            if (string.IsNullOrWhiteSpace(lv2Code)) {
                result = items.Where(o => o.Lv1 == lv1Code && !string.IsNullOrWhiteSpace(o.Lv2) && string.IsNullOrWhiteSpace(o.Lv3))
                    .Select(x => new Unit {
                        Code = x.Lv2!,
                        Name = x.Name,
                    }).ToList();
                return await Task.FromResult(result);
            }

            result = items.Where(o => o.Lv1 == lv1Code && o.Lv2 == lv2Code && !string.IsNullOrWhiteSpace(o.Lv3))
                .Select(o => new Unit {
                    Code = o.Lv3!,
                    Name = o.Name,
                }).ToList();
            return await Task.FromResult(result);
        }

        public Dictionary<string, Unit> GetByCode(params string?[] codes) {
            codes = codes.Where(code => !string.IsNullOrWhiteSpace(code)).ToArray();
            if (codes.Length == 0) return new Dictionary<string, Unit>();

            var lv1Items = items.Where(o => codes.Contains(o.Lv1) && string.IsNullOrWhiteSpace(o.Lv2) && string.IsNullOrWhiteSpace(o.Lv3))
                .Select(o => new Unit {
                    Code = o.Lv1,
                    Name = o.Name,
                }).ToList();

            var lv2Items = items.Where(o => !string.IsNullOrWhiteSpace(o.Lv1) && codes.Contains(o.Lv2) && string.IsNullOrWhiteSpace(o.Lv3))
                .Select(o => new Unit {
                    Code = o.Lv2!,
                    Name = o.Name,
                }).ToList();

            var lv3Items = items.Where(o => !string.IsNullOrWhiteSpace(o.Lv1) && !string.IsNullOrWhiteSpace(o.Lv2) && codes.Contains(o.Lv3))
                .Select(o => new Unit {
                    Code = o.Lv3!,
                    Name = o.Name,
                }).ToList();

            return lv1Items.Union(lv2Items).Union(lv3Items).ToDictionary(k => k.Code);
        }

        public Unit? GetByCode(string? code) {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            return items.Where(o => o.Lv1 == code || o.Lv2 == code || o.Lv3 == code).Select(o => new Unit {
                Code = code,
                Name = o.Name,
            }).FirstOrDefault();
        }

        public List<AddressMap> GetByName(params string?[] names) {
            var tempNames = names.Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => StringHelper.ReplaceSpace(name!.Replace("-", " "), true))
                .ToList();
            if (tempNames.Count == 0) return new List<AddressMap>();

            var data = items.Where(o => {
                var tempName = StringHelper.ReplaceSpace(o.Name.Replace("-", " "), true);
                return tempNames.Contains(tempName);
            }).Select(o => new Unit {
                Code = o.Lv3 ?? o.Lv2 ?? o.Lv1,
                Name = o.Name,
            }).ToList();

            return data.Select(o => new AddressMap() {
                Name = StringHelper.ReplaceSpace(o.Name.Replace("-", " "), true),
                Unit = o
            }).ToList();
        }
    }
}