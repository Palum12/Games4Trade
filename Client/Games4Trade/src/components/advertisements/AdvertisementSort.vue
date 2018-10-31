<template>
    <div>
        <form class="form-inline">
                    <label for="sort">Sortuj według</label>
                    <select
                            class="form-control mx-2"
                            id="sort"
                            v-model="sortCriteria.sort">
                        <option value="Price">Cena</option>
                        <option value="DateCreated">Data dodania ogłoszenia</option>
                    </select>
                    <input type="checkbox" id="desc" v-model="sortCriteria.desc">
                    <label for="desc">Malejąco</label>
                    <button type="button" class="btn btn-outline-success mx-2" @click="applySort">Sortuj</button>
        </form>
    </div>
</template>

<script>
export default {
  name: 'AdvertisementSort',
  props: ['sortCriteria'],
  methods: {
    applySort () {
      this.$emit('filter')
    }
  }
}
</script>

<style scoped>

</style>
