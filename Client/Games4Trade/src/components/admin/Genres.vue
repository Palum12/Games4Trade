<template>
    <div class="form rounded m-2 p-3">
        <form v-on:submit.prevent>
            <h2 class="text-center">Gatunki</h2>
            <div v-for="genre in genres" :key="genre.id">
                <div class="form-row text-center mb-1">
                    <div class=" col-lg-8 col-md-6 col-12">
                        <input
                                type="text"
                                id="key"
                                class="form-control"
                                v-model="genre.value"
                        >
                    </div>
                    <div v-if="shouldSave(genre.id)" class="col-lg-4 col-md-6 col-12 d-flex justify-content-end">
                        <button class="btn btn-info" @click="save(genre)">Zapisz</button>
                    </div>
                    <div v-else class="col-lg-4 col-md-6 col-12 d-flex justify-content-end">
                        <button class="btn btn-warning" @click="modify(genre)">Modyfikuj</button>
                        <button class="btn btn-danger ml-1" @click="remove(genre.id)">X</button>
                    </div>
                </div>
            </div>

            <button class="btn btn-info btn-block" :disabled="!canAdd" @click="addPlace">Dodaj nowy gatunek</button>

        </form>
    </div>
</template>

<script>
export default {
  name: 'Genres',
  data () {
    return {
      isDbInSync: true,
      genres: [{
        id: 3,
        value: 'Wyścigi'
      },
      {
        id: 4,
        value: 'FPS'
      }]
    }
  },
  computed: {
    canAdd () {
      return !this.genres.some(genre => genre.value === '') && this.isDbInSync
    }
  },
  methods: {
    addPlace () {
      if (this.genres.length > 0) {
        this.genres.push({id: this.genres[this.genres.length - 1].id + 1, value: ''})
      } else {
        this.genres.push({id: 0, value: ''})
      }
      this.isDbInSync = false
    },
    save (genre) {
      console.log('test')
      this.isDbInSync = true
    },
    modify (genre) {
      console.log(genre)
    },
    remove (genreId) {
      console.log(genreId)
      this.genres = this.genres.filter(genre => {
        return genre.id !== genreId
      })
    },
    shouldSave (id) {
      let isLast
      if (this.genres.length > 0) {
        isLast = this.genres[this.genres.length - 1].id === id
      } else {
        isLast = true
      }
      return !this.isDbInSync && isLast
    }
  },
  mounted () {
    console.log('test')
  }
}
</script>

<style scoped>

</style>
